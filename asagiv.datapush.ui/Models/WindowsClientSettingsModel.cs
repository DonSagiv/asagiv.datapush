using asagiv.datapush.ui.common.Models;
using System.Windows;

namespace asagiv.datapush.ui.Models
{
    public sealed class WindowsClientSettingsModel : ClientSettingsModelBase
    {
        protected override bool IsPullNodeSelected()
        {
            if (!base.IsPullNodeSelected())
            {
                MessageBox.Show("Please select a destination.",
                    "No destination selected.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return false;
            }

            return true;
        }

        protected override bool IsConnectionSettingSelected()
        {
            if (!base.IsConnectionSettingSelected())
            {
                MessageBox.Show("Please select a Connection Setting.",
                    "Connection Setting not selcted.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return false;
            }

            return true;
        }
    }
}
