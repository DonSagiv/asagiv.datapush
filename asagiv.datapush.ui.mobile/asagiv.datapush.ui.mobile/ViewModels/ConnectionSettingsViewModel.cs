using asagiv.datapush.common.Interfaces;
using asagiv.datapush.common.Models;
using asagiv.datapush.ui.mobile.Utilities;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace asagiv.datapush.ui.mobile.ViewModels
{
    public class ConnectionSettingsViewModel : ReactiveObject
    {
        #region Fields
        private IClientConnectionSettings _selectedConnectionSettings;
        #endregion

        #region Properties
        public ObservableCollection<IClientConnectionSettings> ConnectionSettingsList { get; }
        public IClientConnectionSettings SelectedConnectionSettings
        {
            get => _selectedConnectionSettings;
            set => this.RaiseAndSetIfChanged(ref _selectedConnectionSettings, value);
        }
        #endregion

        #region Commands
        public ICommand NewConnectionSettingCommand { get; }
        public ICommand DeleteConnectionSettingCommand { get; }
        public ICommand SaveConnectionSettingCommand { get; }
        #endregion

        #region Constructor
        public ConnectionSettingsViewModel()
        {
            ConnectionSettingsList = new ObservableCollection<IClientConnectionSettings>();

            NewConnectionSettingCommand = ReactiveCommand.Create(() => CreateNewConnectionSetting());
            DeleteConnectionSettingCommand = ReactiveCommand.Create(() => DeleteConnectionSetting());
            SaveConnectionSettingCommand = ReactiveCommand.Create(async () => await SaveConnectionSettingAsync());
        }
        #endregion

        #region Methods
        public async Task RefreshConnectionsAsync()
        {
            ConnectionSettingsList.Clear();

            var connectionSettingsToAdd = await XFormsDataPushDbContext.Instance.ConnectionSettingsSet
                .OrderBy(x => x.ConnectionName)
                .ToListAsync();

            foreach (var item in connectionSettingsToAdd)
            {
                ConnectionSettingsList.Add(item);
            }
        }

        private void CreateNewConnectionSetting()
        {
            var newConnectionSetting = new ClientConnectionSettings
            {
                ConnectionName = "New Connection Settings"
            };

            ConnectionSettingsList.Add(newConnectionSetting);

            SelectedConnectionSettings = newConnectionSetting;
        }

        public void DeleteConnectionSetting()
        {

        }

        public Task SaveConnectionSettingAsync()
        {
            return Task.CompletedTask;
        }
        #endregion
    }
}
