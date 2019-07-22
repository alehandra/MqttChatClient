using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace MqttChatClient
{
    public class Friend : INotifyPropertyChanged
    {

        #region Fields

        private Color _nameColor = Color.Gray;
        private string _status = "offline";
        private Color _statusColor = Color.LightGray;

        #endregion Fields

        #region Properties

        public string Name { get; set; }

        public Color NameColor
        {
            get
            {
                return _nameColor;
            }
            set
            {
                _nameColor = value;
                RaisePropertyChanged("NameColor");
            }
        }

        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;

                if (_status.Equals("online"))
                    StatusColor = Color.GreenYellow;
                else
                   StatusColor = Color.LightGray;

                RaisePropertyChanged("Status");
            }
        }

        public Color StatusColor
        {
            get
            {
                return _statusColor;
            }
            set
            {
                _statusColor = value;
                RaisePropertyChanged("StatusColor");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Properties

        #region Constructors

        #endregion Constructors

        #region Methods

        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public void SetBlackNameColor()
        {
            NameColor = Color.Black;
        }

        public void SetGrayNameColor()
        {
            NameColor = Color.Gray;
        }
        #endregion Methods
    }
}
