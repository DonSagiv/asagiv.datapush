using asagiv.datapush.ui.Models;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace asagiv.datapush.ui.Utilities
{
    public class ServiceStatusValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is WinServiceStatus statusValue)
            {
                return statusValue switch
                {
                    WinServiceStatus.NotInstalled => Brushes.DarkRed,
                    WinServiceStatus.Stopped => Brushes.Orange,
                    WinServiceStatus.Running => Brushes.LimeGreen,
                    WinServiceStatus.Error => Brushes.Red,
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
