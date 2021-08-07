using asagiv.datapush.common.Models;
using asagiv.datapush.ui.Utilities;
using Microsoft.EntityFrameworkCore;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

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

        #region Constructor
        public ConnectionSettingsViewModel()
        {
            ClientConnectionSettingsList = new ObservableCollection<ClientConnectionSettings>();
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

            var clientConnectionSettingsToAdd = await WinUiDataPushDbContext.Instance.ConnectionSettingsSet.ToListAsync();

            ClientConnectionSettingsList.AddRange(clientConnectionSettingsToAdd);
        }
        #endregion
    }
}
