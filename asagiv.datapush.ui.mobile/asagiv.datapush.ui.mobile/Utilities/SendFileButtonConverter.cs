using System;
using System.Globalization;
using Xamarin.Forms;

namespace asagiv.datapush.ui.mobile.Utilities
{
    public class SendFileButtonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool bValue && bValue
                ? "Share Data"
                : "Select Files to Send";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
