using asagiv.datapush.common.Interfaces;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace asagiv.datapush.ui.common.Interfaces
{
    public interface IConnectionSettingsViewModel
    {
        #region Properties
        ObservableCollection<IClientConnectionSettings> ClientConnectionSettingsList { get; }
        IClientConnectionSettings SelectedClientConnection { get; }
        #endregion

        #region Methods
        Task RefreshConnectionSettingsAsync();
        void CreateNewConnectionSettings();
        Task SaveConnectionSettingsAsync();
        Task DeleteConnectionSettingsAsync();
        #endregion
    }
}
