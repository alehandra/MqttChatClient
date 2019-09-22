using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;
using MqttChatClient.Models;
using System.Windows.Input;

namespace MqttChatClient.ViewModels
{
    public class MessagingViewModel : ContactBase
    {
        #region Fields

        private ObservableCollection<MessageWrapper> _messageList;
        private string _messageEntry;

        #endregion Fields

        #region Properties

        public PhoneContact Contact { get; set; }

        public ObservableCollection<MessageWrapper> MessageList

        {
            get
            {
                return _messageList;
            }
            set
            {
                _messageList = value;
                RaisePropertyChanged("MessageList");
            }
        }

        public string MessageEntry
        {
            get
            {
                return _messageEntry;
            }
            set
            {
                _messageEntry = value;
                RaisePropertyChanged("MessageEntry");
            }
        }
      
        public ICommand SendMessageCommand { get; private set; }

        public Action RefreshScrollDown;

        #endregion Properties

        #region Constructors

        public MessagingViewModel(PhoneContact contact)
        {
            Contact = contact;
            SendMessageCommand = new Command(SendMessage);
            MessageList = new ObservableCollection<MessageWrapper>();
            ManageMessagingCenter();
            ReadMessages();
        }

        #endregion Constructors

        #region Methods

        private void ManageMessagingCenter()
        {
            MessagingCenter.Subscribe<App, Message>(this, "newMessageReceived", (app, message) =>
            {
               if (message.Sender.Equals(Contact.PhoneNumber))
                {
                    message.Status = MessageStatus.Read;
                    MessageWrapper me = new MessageWrapper()
                    {
                        Id = message.Id,
                        Sender = message.Sender,
                        Receiver = message.Receiver,
                        Text = message.Text,
                        CreatedTime = message.CreatedTime,
                        Status = message.Status
                    };

                    MessageList.Add(me);
                    RefreshScrollDown();
                }
            });
        }

        private async void ReadMessages()
        {
            // Consider loading more messages on scroll...
            List<MessageWrapper> l = await App.AppDataBase.GetMessagesAsync(Contact.PhoneNumber, App.Instance.PhoneNumber);
            MessageList = new ObservableCollection<MessageWrapper>(l);
            SetAllMessageStatusesToRead();
        }

        private async void SetAllMessageStatusesToRead()
        {
            List<Message> messagesToUpdate = new List<Message>();

            foreach(MessageWrapper m in MessageList)
            {
                if (m.Status.Equals(MessageStatus.NotRead))
                {
                    m.Status = MessageStatus.Read;
                    messagesToUpdate.Add(m.GetMessage());
                }
            }

            foreach (Message m in messagesToUpdate)
                await App.AppDataBase.SaveItemAsync(m);

            MessagingCenter.Send(this, "messagesRead", Contact);
        }

        private void SendMessage()
        {
            MessageWrapper message = new MessageWrapper() { Text = MessageEntry, Receiver = Contact.PhoneNumber, Sender = App.Instance.PhoneNumber, CreatedTime = DateTime.Now, Status = MessageStatus.Read};
            MessageEntry = string.Empty;
            App.Instance.PublishNewMessageAsync(message.GetMessage());
            MessageList.Add(message);
            RefreshScrollDown();
            MessagingCenter.Send(this, "newMessageSent", Contact);
        }

        #endregion Methods
    }
}
