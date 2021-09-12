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
        public ClientSettingsViewModel DataPushViewModel { get; }
        public ConnectionSettingsViewModel ConnectionSettingsViewModel { get; }
        #endregion

        public MainPageViewModel(ClientSettingsViewModel clientSettingsViewModel, ConnectionSettingsViewModel connectionSettingsViewModel, ILogger logger)
        {
            _logger = logger;

            DataPushViewModel = clientSettingsViewModel;
            ConnectionSettingsViewModel = connectionSettingsViewModel;
        }
    }
}
