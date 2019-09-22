using System;
using System.Collections.Generic;
using System.Text;

namespace MqttChatClient
{
    public interface IContactService
    {
        IEnumerable<PhoneContact> GetAllContacts();

        string GetCurrentPhoneNumberCorrectFormat(string phoneNumber);

        void ShowPopUpForCurrentPhoneNumber(PopupEntry pe);
    }
}
