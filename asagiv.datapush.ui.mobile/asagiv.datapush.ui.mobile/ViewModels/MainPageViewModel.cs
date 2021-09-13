using asagiv.datapush.ui.common.Interfaces;
using ReactiveUI;
using Serilog;

namespace asagiv.datapush.ui.mobile.ViewModels
{
    public class MainPageViewModel : ReactiveObject
    {
        #region Fields
        private readonly ILogger _logger;
        #endregion

        #region ViewModels
        public IClientSettingsViewModel DataPushViewModel { get; }
        public IConnectionSettingsViewModel ConnectionSettingsViewModel { get; }
        #endregion

        public MainPageViewModel(IClientSettingsViewModel clientSettingsViewModel, IConnectionSettingsViewModel connectionSettingsViewModel, ILogger logger)
        {
            _logger = logger;

            DataPushViewModel = clientSettingsViewModel;
            ConnectionSettingsViewModel = connectionSettingsViewModel;
        }
    }
}
