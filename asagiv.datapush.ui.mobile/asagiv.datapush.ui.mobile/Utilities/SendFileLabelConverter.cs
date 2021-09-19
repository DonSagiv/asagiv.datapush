using System;
using System.Globalization;
using Xamarin.Forms;

namespace asagiv.datapush.ui.mobile.Utilities
{
    public class SendFileLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool bValue && bValue 
                ? "Share Data To" 
                : "Send File To";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
