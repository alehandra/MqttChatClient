using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MqttChatClient.Models;

namespace MqttChatClient
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : TabbedPage
    {
        private bool isInitialized = false;

        public HomePage ()
        {
            InitializeComponent();
            Children.Add(new ChatsPage());
            Children.Add(new ContactsPage());
            BarBackgroundColor = Color.FromHex("#17445e");
            SelectedTabColor = Color.Azure;
            UnselectedTabColor = Color.FromHex("#729db5");
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (!isInitialized)
            {
              await Task.Run(() =>
                {
                    App.Instance.InitializeWorkingResources();
                    isInitialized = true;
                });
                
            }
        }
    }
}