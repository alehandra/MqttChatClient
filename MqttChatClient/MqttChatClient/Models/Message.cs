using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace MqttChatClient.Models
{
    public enum MessageStatus
    {
        Read = 0,
        NotRead = 1
    }

    public class Message
    {
        #region Properties

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Text { get; set; }

        public string Sender { get; set; }

        public string Receiver { get; set; }

        public DateTime CreatedTime { get; set; }

        public MessageStatus Status { get; set; }

        #endregion Properties

        #region Constructor

        #endregion Constructor
    }
}
