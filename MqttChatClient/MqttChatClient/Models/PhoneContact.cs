using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MqttChatClient
{
    public class PhoneContact : ContactBase
    {
        #region Fields

        #endregion Fields

        #region Properties

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public ImageSource ImageSource { get; set; }

        public string Name { get => $"{FirstName} {LastName}"; }

        #endregion Properties
    }
}

