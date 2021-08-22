using asagiv.datapush.common.Interfaces;
using asagiv.datapush.ui.mobile.Utilities;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

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

        #region Constructor
        public ConnectionSettingsViewModel()
        {
            ConnectionSettingsList = new ObservableCollection<IClientConnectionSettings>();
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
        #endregion
    }
}
