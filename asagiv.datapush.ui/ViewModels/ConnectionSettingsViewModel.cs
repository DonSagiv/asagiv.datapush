using asagiv.datapush.common.Models;
using asagiv.datapush.ui.Utilities;
using Prism.Mvvm;

namespace asagiv.datapush.ui.ViewModels
{
    public class ConnectionSettingsViewModel : BindableBase
    {
        #region Constructo4
        public ConnectionSettingsViewModel() 
        {
            WinUiDataPushDbContext.Instance.ConnectionSettingsSet.Add(new ClientConnectionSettings("192.168.4.23", false));
        }
        #endregion
    }
}
