using asagiv.datapush.ui.Models;
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
        public WindowsClientViewModel WindowsClientViewModel { get; }
        public ConnectionSettingsViewModel ConnectionSettingsViewModel { get; }
        public WindowsServiceViewModel WindowsServiceViewModel { get; }
        #endregion

        #region Constructor
        public MainWindowViewModel(WindowsClientViewModel windowsClientViewModel, ConnectionSettingsViewModel connectionSettingsViewModel, WindowsServiceViewModel windowsServiceViewModel, ILogger logger)
        {
            _logger = logger;
            
            WindowsClientViewModel = windowsClientViewModel;
            ConnectionSettingsViewModel = connectionSettingsViewModel;
            WindowsServiceViewModel = windowsServiceViewModel;
        }
        #endregion
    }
}