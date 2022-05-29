using asagiv.pushrocket.common.Models;
using asagiv.pushrocket.common.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace asagiv.pushrocket.ui.common.Interfaces
{
    public interface IClientSettingsViewModel
    {
        #region Events
        event EventHandler<string> ErrorOccurred;
        #endregion

        #region Properties
        ObservableCollection<IClientConnectionSettings> ConnectionSettingsList { get; }
        ObservableCollection<IDataPushContextViewModel> PushContextList { get; }
        ObservableCollection<string> DestinationNodes { get; }
        #endregion

        #region Methods
        Task RefreshConnectionSettingsAsync();
        Task ConnectToServerAsync();
        ValueTask UploadFilesAsync();
        #endregion
    }
}
