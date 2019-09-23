using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

using Xamarin.Forms;

namespace MqttChatClient
{
    public class ContactsViewModel : ContactBase
    {
        #region Fields

        private INavigation _navigation;
        private ObservableCollection<PhoneContact> _contactList;
        private IEnumerable<PhoneContact> _searchContactList;
        private string _searchString;
        private bool _isBusy;
        private bool _elementsVisible;

        #endregion Fields

        #region Properties

        public ObservableCollection<PhoneContact> ContactList
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

        public IEnumerable<PhoneContact> SearchContactList
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

        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            set
            {
                _isBusy = value;
                RaisePropertyChanged("IsBusy");
            }
        }

        public bool ElementsVisible
        {
            get
            {
                return _elementsVisible;
            }
            set
            {
                _elementsVisible = value;
                RaisePropertyChanged("ElementsVisible");
            }
        }

        #endregion Properties

        #region Constructor

        public ContactsViewModel(INavigation navigation)
        {
            _navigation = navigation;
            ContactList = new ObservableCollection<PhoneContact>(App.Instance.Contacts);
            SearchContactList = ContactList;
            ManageMessagingCenter();
        }

        #endregion Constructor 

        #region Methods

        private void ManageMessagingCenter()
        {
            MessagingCenter.Subscribe<App>(this, "resourcesInitialized", (app) =>
            {
                ContactList =  new ObservableCollection<PhoneContact>(App.Instance.Contacts);
                SearchContactList = ContactList;
                IsBusy = false;
                ElementsVisible = true;
            });
        }
        
        #endregion Methods
        }
}
