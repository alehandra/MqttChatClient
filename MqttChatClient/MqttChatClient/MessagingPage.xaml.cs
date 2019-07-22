using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MqttChatClient
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MessagingPage : ContentPage
    {

        #region Properties

        public Friend MessageFriend { get; set; }

        #endregion Properties

        #region Constructors

        public MessagingPage(Friend friend)
        {
            InitializeComponent();

            MessageFriend = friend;
        }

        #endregion Constructors

        #region Methods


        public void DisplayNewMessage(string message)
        {
            ExchangedMessages.Text += message;
        }

        #region EventHandlers

        private void SendButton_Clicked(object sender, EventArgs e)
        {
            string messageToSend = MessageEntry.Text;
            ExchangedMessages.Text += "\n" + messageToSend;
            MessageEntry.Text = "";

            App.Instance.PublishNewMessage(MessageFriend, "Message", messageToSend, false);
        }

        private void RemoveButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
            App.Instance.RemoveFriend(MessageFriend);
            
        }

        #endregion EventHandlers

        #endregion Methods
    }
}