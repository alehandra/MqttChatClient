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
        public HomePage ()
        {
            InitializeComponent();
            Children.Add(new ChatsPage());
            Children.Add(new ContactsPage());
            BarBackgroundColor = Color.FromHex("#17445e");
            SelectedTabColor = Color.Azure;
            UnselectedTabColor = Color.FromHex("#729db5");
        }
    }
}