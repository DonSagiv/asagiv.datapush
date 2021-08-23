using asagiv.datapush.common.Interfaces;
using asagiv.datapush.common.Models;
using asagiv.datapush.common.Utilities;
using DynamicData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace asagiv.datapush.ui.common
{
    public abstract class ConnectionSettingsViewModelBase : ReactiveObject
    {
        #region Fields
        protected ClientConnectionSettings _selectedClientConnection;
        protected DataPushDbContextBase _dataPushDbContext;
        #endregion

        #region Properties
        public ObservableCollection<ClientConnectionSettings> ClientConnectionSettingsList { get; }
        public ClientConnectionSettings SelectedClientConnection
        {
            get { return _selectedClientConnection; }
            set { this.RaiseAndSetIfChanged(ref _selectedClientConnection, value); }
        }
        #endregion

        #region Commands
        public ICommand NewConnectionSettingsCommand { get; }
        public ICommand SaveConnectionSettingsCommand { get; }
        public ICommand DeleteConnectionSettingsCommand { get; }
        #endregion

        #region Constructor
        protected ConnectionSettingsViewModelBase(DataPushDbContextBase dataPushDbContext)
        {
            _dataPushDbContext = dataPushDbContext;

            ClientConnectionSettingsList = new ObservableCollection<ClientConnectionSettings>();

            NewConnectionSettingsCommand = ReactiveCommand.Create(() => CreateNewConnectionSettings());
            SaveConnectionSettingsCommand = ReactiveCommand.Create(() => SaveConnectionSettingsAsync());
            DeleteConnectionSettingsCommand = ReactiveCommand.Create(async () => await DeleteConnectionSettingsAsync());
        }
        #endregion

        #region Methods
        public async Task RefreshConnectionSettingsAsync()
        {
            ClientConnectionSettingsList.Clear();

            var clientConnectionSettingsToAdd = await _dataPushDbContext.ConnectionSettingsSet
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

        private async Task SaveConnectionSettingsAsync()
        {
            if (ConnectionStringIsNullOrEmpty(_selectedClientConnection.ConnectionString) || SettingsListHasName())
            {
                return;
            }

            EntityEntry<ClientConnectionSettings> result;

            if (await _dataPushDbContext.ConnectionSettingsSet.ContainsAsync(_selectedClientConnection))
            {
                result = _dataPushDbContext.ConnectionSettingsSet.Update(_selectedClientConnection);
            }
            else
            {
                result = await _dataPushDbContext.ConnectionSettingsSet.AddAsync(_selectedClientConnection);
            }

            await _dataPushDbContext.SaveChangesAsync();
        }

        private async Task DeleteConnectionSettingsAsync()
        {
            if (_selectedClientConnection == null || !ConfirmUserDelete())
            {
                return;
            }

            if (_dataPushDbContext.ConnectionSettingsSet.Contains(_selectedClientConnection))
            {
                _dataPushDbContext.ConnectionSettingsSet.Remove(_selectedClientConnection);

                await _dataPushDbContext.SaveChangesAsync();
            }

            ClientConnectionSettingsList.Remove(_selectedClientConnection);
        }

        protected virtual bool SettingsListHasName()
        {
            return ClientConnectionSettingsList
                .Where(x => x != _selectedClientConnection)
                .Any(x => x.ConnectionName == _selectedClientConnection.ConnectionName);
        }

        protected virtual bool ConnectionStringIsNullOrEmpty(string connectionString)
        {
            return string.IsNullOrWhiteSpace(connectionString);
        }

        protected virtual bool ConfirmUserDelete()
        {
            return true;
        }
        #endregion

    }
}
