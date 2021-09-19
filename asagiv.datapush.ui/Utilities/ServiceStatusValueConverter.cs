using asagiv.datapush.common.Models;
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
            if(value is PullNodeStatus statusValue)
            {
                return statusValue switch
                {
                    PullNodeStatus.NotInstalled => Brushes.DarkRed,
                    PullNodeStatus.Stopped => Brushes.Orange,
                    PullNodeStatus.Running => Brushes.LimeGreen,
                    PullNodeStatus.Error => Brushes.Red,
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
