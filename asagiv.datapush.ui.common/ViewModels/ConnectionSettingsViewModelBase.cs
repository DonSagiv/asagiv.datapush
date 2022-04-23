using asagiv.datapush.common.Interfaces;
using asagiv.datapush.common.Models;
using asagiv.datapush.common.Utilities;
using asagiv.datapush.ui.common.Interfaces;
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
    public abstract class ConnectionSettingsViewModelBase : ReactiveObject, IConnectionSettingsViewModel
    {
        #region Fields
        protected IClientConnectionSettings _selectedClientConnection;
        protected DataPushDbContextBase _dataPushDbContext;
        #endregion

        #region Properties
        public ObservableCollection<IClientConnectionSettings> ClientConnectionSettingsList { get; }
        public IClientConnectionSettings SelectedClientConnection
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

            ClientConnectionSettingsList = new ObservableCollection<IClientConnectionSettings>();

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

        public void CreateNewConnectionSettings()
        {
            var settingToAdd = new ClientConnectionSettings
            {
                ConnectionName = "New Connection Settings"
            };

            ClientConnectionSettingsList.Add(settingToAdd);

            SelectedClientConnection = settingToAdd;
        }

        public async Task SaveConnectionSettingsAsync()
        {
            if (ConnectionStringIsNullOrEmpty(_selectedClientConnection.ConnectionString) || SettingsListHasName())
            {
                return;
            }

            EntityEntry<ClientConnectionSettings> result;

            var clientSetting = _selectedClientConnection as ClientConnectionSettings;

            if (await _dataPushDbContext.ConnectionSettingsSet.AnyAsync(x => x.Id == _selectedClientConnection.Id))
            {
                result = _dataPushDbContext.ConnectionSettingsSet.Update(clientSetting);
            }
            else
            {
                result = await _dataPushDbContext.ConnectionSettingsSet.AddAsync(clientSetting);
            }

            await _dataPushDbContext.SaveChangesAsync();
        }

        public async Task DeleteConnectionSettingsAsync()
        {
            if (_selectedClientConnection == null || !ConfirmUserDelete())
            {
                return;
            }

            var clientSetting = _selectedClientConnection as ClientConnectionSettings;

            if (_dataPushDbContext.ConnectionSettingsSet.Contains(_selectedClientConnection))
            {
                _dataPushDbContext.ConnectionSettingsSet.Remove(clientSetting);

                await _dataPushDbContext.SaveChangesAsync();
            }

            ClientConnectionSettingsList.Remove(_selectedClientConnection);
        }

        protected virtual bool SettingsListHasName()
        {
            return ClientConnectionSettingsList
                .Any(x => x != _selectedClientConnection && x.ConnectionName == _selectedClientConnection.ConnectionName)
;
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
