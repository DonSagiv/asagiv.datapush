using asagiv.datapush.common.Interfaces;

namespace asagiv.datapush.ui.common.Interfaces
{
    public interface IPullNodeSettingsViewModel
    {
        #region Properties
        IPullNodeSettingsModel ServiceModel { get; }
        #endregion
    }
}
