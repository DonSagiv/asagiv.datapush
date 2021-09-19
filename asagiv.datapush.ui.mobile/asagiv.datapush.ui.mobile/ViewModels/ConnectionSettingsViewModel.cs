using asagiv.datapush.ui.common;
using asagiv.datapush.ui.mobile.Utilities;

namespace asagiv.datapush.ui.mobile.ViewModels
{
    public class ConnectionSettingsViewModel : ConnectionSettingsViewModelBase
    {
        #region Constructor
        public ConnectionSettingsViewModel(XFormsDataPushDbContext dbContext) : base(dbContext) { }
        #endregion
    }
}
