using asagiv.pushrocket.common.Interfaces;
using System;

namespace asagiv.pushrocket.ui.common.Interfaces
{
    public interface IPullNodeSettingsViewModel : IDisposable
    {
        #region Properties
        IPullNodeSettingsModel ServiceModel { get; }
        #endregion
    }
}
