using System;
using System.Collections;
using System.Globalization;
using Xamarin.Forms;

namespace asagiv.datapush.ui.mobile.Utilities
{
    public class AnyToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var enumerable = value as ICollection;

            return enumerable.Count > 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
