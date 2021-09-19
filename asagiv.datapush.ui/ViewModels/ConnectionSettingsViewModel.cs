using asagiv.datapush.ui.common;
using asagiv.datapush.ui.Utilities;
using System.Windows;

namespace asagiv.datapush.ui.ViewModels
{
    public class ConnectionSettingsViewModel : ConnectionSettingsViewModelBase
    {
        #region Constructor
        public ConnectionSettingsViewModel(WinUiDataPushDbContext dbContext) : base(dbContext) { }
        #endregion

        #region Methods
        protected override bool SettingsListHasName()
        {
            if (base.SettingsListHasName())
            {
                MessageBox.Show($"There is already a setting called {_selectedClientConnection.ConnectionName}",
                    "Existing Setting",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return true;
            }

            return false;
        }

        protected override bool ConnectionStringIsNullOrEmpty(string connectionString)
        {
            if (base.ConnectionStringIsNullOrEmpty(connectionString))
            {
                MessageBox.Show($"Please enter a value for the Connection String.",
                    "Connection String Empty",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return true;
            }

            return false;
        }

        protected override bool ConfirmUserDelete()
        {
            var result = MessageBox.Show("Are you sure you want to delete this connection setting?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            return result == MessageBoxResult.Yes;
        }
        #endregion
    }
}
