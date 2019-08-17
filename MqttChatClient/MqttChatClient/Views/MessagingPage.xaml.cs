using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MqttChatClient.ViewModels;
using MqttChatClient.Models;

namespace MqttChatClient
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MessagingPage : ContentPage
	{
        private MessagingViewModel ViewModel
        {
            get { return BindingContext as MessagingViewModel; }
        }

        public MessagingPage(PhoneContact contact)
		{
			InitializeComponent ();
            BindingContext = new MessagingViewModel(contact);     
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            var myListView = (ListView)this.FindByName("MessageListView");

            
            ViewModel.RefreshScrollDown = () => {
                if (ViewModel.MessageList.Count > 0)
                {
                    Device.BeginInvokeOnMainThread(() => 
                    {

                        myListView.ScrollTo(ViewModel.MessageList[ViewModel.MessageList.Count - 1], ScrollToPosition.End, true);
                    });
                }
            };

            ViewModel.RefreshScrollDown();
        }
    }
}