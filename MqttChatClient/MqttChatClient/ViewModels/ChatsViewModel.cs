using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using MqttChatClient.Models;

namespace MqttChatClient.ViewModels
{
    public class ChatsViewModel : ContactBase
    {
        #region Fields

        private INavigation _navigation;
        private ObservableCollection<PhoneContactWrapper> _contactList;
        private IEnumerable<PhoneContactWrapper> _searchContactList;
        private string _searchString;

        #endregion Fields

        #region Properties

        public ObservableCollection<PhoneContactWrapper> ContactList
        {
            get
            {
                return _contactList;
            }
            set
            {
                _contactList = value;
                RaisePropertyChanged("ContactList");
            }
        }

        public IEnumerable<PhoneContactWrapper> SearchContactList
        {
            get
            {
                return _searchContactList;
            }
            set
            {
                _searchContactList = value;
                RaisePropertyChanged("SearchContactList");
            }
        }

        public string SearchString
        {
            get
            {
                return _searchString;
            }
            set
            {
                _searchString = value;
                SearchContactList = string.IsNullOrEmpty(value) ? ContactList : ContactList.Where(i => i.Name.ToLower().Contains(value.ToLower()));
                RaisePropertyChanged("SearchString");
            }
        }


        #endregion Properties

        #region Constructors

        public ChatsViewModel(INavigation navigation)
        {
            _navigation = navigation;
            ContactList = new ObservableCollection<PhoneContactWrapper>();
            ManageMessagingCenter();
        }

        #endregion Constructors

        #region Methods

        private void ManageMessagingCenter()
        {
            MessagingCenter.Subscribe<App>(this, "resourcesInitialized", (app) => 
            {
                ReadLatestMessagesAndFillChat();
            });
            MessagingCenter.Subscribe<App, Message>(this, "newMessageReceived", (app, message) =>
            {
                var temporary = ContactList.Where(pc => pc.PhoneNumber.Equals(message.Sender));
                if (temporary != null && temporary.Count() > 0)
                {
                    PhoneContactWrapper pc = temporary.First();
                    pc.HasUnreadMessages = true;
                    ContactList.Remove(pc);
                    ContactList.Insert(0, pc);
                }
                else
                {
                    var temp = App.Instance.Contacts.Where(pc => pc.PhoneNumber.Equals(message.Sender));
                    if (temp != null && temp.Count() > 0)
                    {
                        PhoneContactWrapper pce = new PhoneContactWrapper(temp.First())
                        {
                            HasUnreadMessages = true
                        };
                        ContactList.Insert(0, pce);
                    }
                    else
                    {
                        // this number is not in the contactlist
                        // add logic for this..
                    }
                }
            });

            MessagingCenter.Subscribe<MessagingViewModel, PhoneContact>(this, "messagesRead", (mvm, contact) =>
            {
                var temp = ContactList.Where(pc => pc.Equals(contact));
                if (temp != null && temp.Count() > 0)
                {
                    PhoneContactWrapper pce = temp.First();
                    pce.HasUnreadMessages = false;
                }

            });

            MessagingCenter.Subscribe<MessagingViewModel, PhoneContact>(this, "newMessageSent", (mvm, contact) =>
            {
                var temp = ContactList.Where(pc => pc.Equals(contact));
                if (temp != null && temp.Count() > 0)
                {
                    PhoneContactWrapper pce = temp.First();
                    pce.HasUnreadMessages = false;
                    ContactList.Remove(pce);
                    ContactList.Insert(0, pce);
                }
                else
                {
                    PhoneContactWrapper pce = new PhoneContactWrapper(contact)
                    {
                        HasUnreadMessages = false
                    };
                    ContactList.Insert(0, pce);
                }
               

            });
        }

        private async void ReadLatestMessagesAndFillChat()
        {
            List<Message> latestMessages = await App.AppDataBase.GetLatestMessagges();
            
            foreach (Message m in latestMessages)
            {
                string currrentUser = App.Instance.PhoneNumber;
                if (m.Receiver.Equals(currrentUser) || m.Sender.Equals(currrentUser))
                {
                    string contactNumber = m.Receiver.Equals(App.Instance.PhoneNumber) ? m.Sender : m.Receiver;

                    if (ContactList.Any(c => c.PhoneNumber.Equals(contactNumber)))
                        continue;

                    IEnumerable<PhoneContact> l = App.Instance.Contacts.Where(c => c.PhoneNumber.Equals(contactNumber));
                    PhoneContact contact = l?.Count() > 0 ? l.First() : null;
                    if (contact != null)
                    {
                        PhoneContactWrapper pe = new PhoneContactWrapper(contact)
                        {
                            HasUnreadMessages = m.Status == MessageStatus.NotRead
                        };
                        ContactList.Add(pe);
                    }

                }
            }
            SearchContactList = ContactList;
        }

        #endregion Methods

    }
}
