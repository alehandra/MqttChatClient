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
    public partial class ContactsPage : ContentPage
    {
        public ContactsPage()
        {
            InitializeComponent();
            BindingContext = new ContactsViewModel(Navigation);
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