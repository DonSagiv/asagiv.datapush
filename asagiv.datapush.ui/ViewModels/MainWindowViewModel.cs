using asagiv.datapush.ui.common.Interfaces;
using ReactiveUI;
using Serilog;

namespace asagiv.datapush.ui.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        #region Fields
        private ILogger _logger;
        #endregion

        #region ViewModels
        public IClientSettingsViewModel ClientSettingsViewModel { get; }
        public IConnectionSettingsViewModel ConnectionSettingsViewModel { get; }
        public IPullNodeSettingsViewModel PullNodeSettingsViewModel { get; }
        #endregion

        #region Constructor
        public MainWindowViewModel(IClientSettingsViewModel clientSettingsViewModel, IConnectionSettingsViewModel connectionSettingsViewModel, IPullNodeSettingsViewModel windowsServiceViewModel, ILogger logger)
        {
            _logger = logger;
            
            ClientSettingsViewModel = clientSettingsViewModel;
            ConnectionSettingsViewModel = connectionSettingsViewModel;
            PullNodeSettingsViewModel = windowsServiceViewModel;
        }
        #endregion
    }
}