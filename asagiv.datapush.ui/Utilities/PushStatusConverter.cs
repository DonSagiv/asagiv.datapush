using asagiv.datapush.common.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace asagiv.datapush.ui.Utilities
{
    class PushStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DeliveryStatus status)
            {
                return status switch
                {
                    DeliveryStatus.None => Brushes.Black,
                    DeliveryStatus.Pending => Brushes.Yellow,
                    DeliveryStatus.InProgress => Brushes.Green,
                    DeliveryStatus.Successful => Brushes.Blue,
                    DeliveryStatus.Failed => Brushes.Red,
                    _ => Brushes.Transparent,
                };
            }

            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
