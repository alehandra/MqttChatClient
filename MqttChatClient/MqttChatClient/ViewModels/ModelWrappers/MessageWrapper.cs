using System;
using System.Collections.Generic;
using System.Text;

namespace MqttChatClient.Models
{
    public class MessageWrapper: Message
    {

        #region Properties

        public string TextAlignment
        {
            get
            {
                return App.Instance.PhoneNumber.Equals(Sender) ? "End" : "Start";
            }
        }

        public string MessageLayout
        {
            get
            {
                return App.Instance.PhoneNumber.Equals(Sender) ? "EndAndExpand" : "StartAndExpand";
            }
        }

        public string Color
        {
            get
            {
                return App.Instance.PhoneNumber.Equals(Sender) ? "#17445e" : "#31586e";
            }
        }


        #endregion Properties

        #region Constructors

        #endregion Constructors

        #region Methods

        public Message GetMessage()
        {
            Message result = new Message()
            {
                Id = this.Id,
                Receiver = this.Receiver,
                Sender = this.Sender,
                Text = this.Text,
                CreatedTime = this.CreatedTime,
                Status = this.Status
            };
            return result;
        }

        #endregion Mehods
    }
}
