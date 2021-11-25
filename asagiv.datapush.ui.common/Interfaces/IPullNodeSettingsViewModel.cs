using asagiv.datapush.common.Interfaces;
using System;

namespace asagiv.datapush.ui.common.Interfaces
{
    public interface IPullNodeSettingsViewModel : IDisposable
    {
        #region Properties
        IPullNodeSettingsModel ServiceModel { get; }
        #endregion
    }
}
