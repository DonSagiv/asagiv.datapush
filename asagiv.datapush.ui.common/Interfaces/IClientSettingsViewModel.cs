using asagiv.datapush.common.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace asagiv.datapush.ui.common.Interfaces
{
    public interface IClientSettingsViewModel
    {
        #region Events
        event EventHandler<string> ErrorOccurred;
        #endregion

        #region Properties
        ObservableCollection<IClientConnectionSettings> ConnectionSettingsList { get; }
        ObservableCollection<IDataPushContext> PushContextList { get; }
        ObservableCollection<string> DestinationNodes { get; }
        #endregion

        #region Methods
        Task RefreshConnectionSettingsAsync();
        Task ConnectToServerAsync();
        ValueTask UploadFilesAsync();
        #endregion
    }
}
