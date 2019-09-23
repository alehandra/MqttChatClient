using System;
using System.Collections.Generic;
using System.Text;

namespace MqttChatClient.Models
{
    public class PhoneContactWrapper : PhoneContact
    {
        #region Fields

        private bool _hasUnreadMessages;

        #endregion Fields

        #region Properties

        public bool HasUnreadMessages
        {
            get
            {
                return _hasUnreadMessages;
            }
            set
            {
                _hasUnreadMessages = value;
                RaisePropertyChanged("HasUnreadMessages");
            }
        }

        #endregion Properties

        #region Constructors
        public PhoneContactWrapper()
        { }

        public PhoneContactWrapper(PhoneContact pc)
        {
            FirstName = pc.FirstName;
            LastName = pc.LastName;
            PhoneNumber = pc.PhoneNumber;
            ImageSource = pc.ImageSource;
        }
    
        #endregion Constructors
    }
}
