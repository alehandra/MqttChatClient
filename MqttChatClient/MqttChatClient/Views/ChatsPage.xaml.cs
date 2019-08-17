using MqttChatClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MqttChatClient
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ChatsPage : ContentPage
	{
		public ChatsPage ()
		{
			InitializeComponent ();
            BindingContext = new ChatsViewModel(Navigation);
        }

        public void OpenContactMessagePage(object sender, ItemTappedEventArgs e)
        {
            var myListView = (ListView)sender;
            PhoneContact myItem = (PhoneContact)myListView.SelectedItem;
            MessagingPage messagePage = new MessagingPage(myItem);
            Navigation.PushAsync(messagePage);
        }
    }
}