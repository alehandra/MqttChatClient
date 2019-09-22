using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xamarin.Forms;

namespace MqttChatClient
{
    public class PopupEntry
    {
        public string Text { get; set; }
        public string Title { get; set; }
        public List<string> Buttons { get; set; }

        public PopupEntry(string title, string text, params string[] buttons)
        {
            Title = title;
            Text = text;
            Buttons = buttons.ToList();
        }

        public PopupEntry(string title, string text) : this(title, text, "OK", "Cancel")
        {
        }

        public event EventHandler<PopupEntryClosedArgs> PopupClosed;

        public void OnPopupClosed(PopupEntryClosedArgs e)
        {
            PopupClosed?.Invoke(this, e);
        }

        public void Show()
        {
            DependencyService.Get<IContactService>().ShowPopUpForCurrentPhoneNumber(this);
        }
    }

    public class PopupEntryClosedArgs : EventArgs
    {
        public string Text { get; set; }
        public string Button { get; set; }
    }
}
