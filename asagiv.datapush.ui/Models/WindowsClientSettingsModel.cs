using asagiv.datapush.ui.common.Models;
using Serilog;
using System.Windows;

namespace asagiv.datapush.ui.Models
{
    public sealed class WindowsClientSettingsModel : ClientSettingsModelBase
    {
        #region Constructor
        public WindowsClientSettingsModel(ILogger logger) : base(logger) { }
        #endregion

        #region Methods
        protected override bool IsConnectionSettingSelected()
        {
            if (!base.IsConnectionSettingSelected())
            {
                // Show message box when no connection setting is selected.
                MessageBox.Show("Please select a Connection Setting.",
                    "Connection Setting not selcted.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return false;
            }

            return true;
        }

        protected override bool IsPullNodeSelected()
        {
            if (!base.IsPullNodeSelected())
            {
                // Show message box when no pull node is selected.
                MessageBox.Show("Please select a destination.",
                    "No destination selected.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return false;
            }

            return true;
        }
        #endregion
    }
}
