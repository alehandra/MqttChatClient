using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace MqttChatClient
{
    public class LayoutOptionsConverter : IValueConverter
    {
        #region IValueConverter Implementation

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            LayoutOptions result = LayoutOptions.Start;
            if (!string.IsNullOrEmpty((string)value) && ((string)value).Equals("EndAndExpand"))
                result = LayoutOptions.EndAndExpand;

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IValueConverter Implementation

    }
}
