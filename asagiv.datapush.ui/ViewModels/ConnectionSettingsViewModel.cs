using asagiv.datapush.common.Models;
using asagiv.datapush.ui.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace asagiv.datapush.ui.ViewModels
{
    public class ConnectionSettingsViewModel : BindableBase
    {
        #region Fields
        private ClientConnectionSettings _selectedClientConnection;
        #endregion

        #region Properties
        public ObservableCollection<ClientConnectionSettings> ClientConnectionSettingsList { get; }
        public ClientConnectionSettings SelectedClientConnection
        {
            get { return _selectedClientConnection; }
            set { _selectedClientConnection = value; RaisePropertyChanged(nameof(SelectedClientConnection)); }
        }
        #endregion

        #region Commands
        public ICommand NewConnectionSettingsCommand { get; }
        public ICommand SaveConnectionSettingsCommand { get; }
        public ICommand DeleteConnectionSettingsCommand { get; }
        #endregion

        #region Constructor
        public ConnectionSettingsViewModel()
        {
            ClientConnectionSettingsList = new ObservableCollection<ClientConnectionSettings>();

            NewConnectionSettingsCommand = new DelegateCommand(CreateNewConnectionSettings);
            SaveConnectionSettingsCommand = new DelegateCommand(SaveConnectionSettingsAsync);
            DeleteConnectionSettingsCommand = new DelegateCommand(DeleteConnectionSettingsAsync);
        }
        #endregion

        #region Methods
        public Task InitializeAsync()
        {
            return RefreshConnectionStringsAsync();
        }

        public async Task RefreshConnectionStringsAsync()
        {
            ClientConnectionSettingsList.Clear();

            var clientConnectionSettingsToAdd = await WinUiDataPushDbContext.Instance.ConnectionSettingsSet
                .OrderBy(x => x.ConnectionName)
                .ToListAsync();

            ClientConnectionSettingsList.AddRange(clientConnectionSettingsToAdd);
        }

        private void CreateNewConnectionSettings()
        {
            var settingToAdd = new ClientConnectionSettings
            {
                ConnectionName = "New Connection Settings"
            };

            ClientConnectionSettingsList.Add(settingToAdd);

            SelectedClientConnection = settingToAdd;
        }

        private async void SaveConnectionSettingsAsync()
        {
            if (ConnectionStringIsNullOrEmpty(_selectedClientConnection.ConnectionString) || SettingsListHasName())
            {
                return;
            }

            EntityEntry<ClientConnectionSettings> result;

            if (await WinUiDataPushDbContext.Instance.ConnectionSettingsSet.ContainsAsync(_selectedClientConnection))
            {
                result = WinUiDataPushDbContext.Instance.ConnectionSettingsSet.Update(_selectedClientConnection);
            }
            else
            {
                result = await WinUiDataPushDbContext.Instance.ConnectionSettingsSet.AddAsync(_selectedClientConnection);
            }

            await WinUiDataPushDbContext.Instance.SaveChangesAsync();
        }

        private async void DeleteConnectionSettingsAsync()
        {
            if (_selectedClientConnection == null || !ConfirmUserDelete())
            {
                return;
            }

            if (WinUiDataPushDbContext.Instance.ConnectionSettingsSet.Contains(_selectedClientConnection))
            {
                WinUiDataPushDbContext.Instance.ConnectionSettingsSet.Remove(_selectedClientConnection);

                await WinUiDataPushDbContext.Instance.SaveChangesAsync();
            }

            ClientConnectionSettingsList.Remove(_selectedClientConnection);
        }

        private bool SettingsListHasName()
        {
            var hasSameName = ClientConnectionSettingsList
                            .Where(x => x != _selectedClientConnection)
                            .Any(x => x.ConnectionName == _selectedClientConnection.ConnectionName);

            if (hasSameName)
            {
                MessageBox.Show($"There is already a setting called {_selectedClientConnection.ConnectionName}",
                    "Existing Setting",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return true;
            }

            return false;
        }

        private static bool ConnectionStringIsNullOrEmpty(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                MessageBox.Show($"Please enter a value for the Connection String.",
                    "Connection String Empty",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return true;
            }

            return false;
        }

        private static bool ConfirmUserDelete()
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
