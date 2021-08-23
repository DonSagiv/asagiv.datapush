using asagiv.datapush.ui.Models;
using ReactiveUI;

namespace asagiv.datapush.ui.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        #region ViewModels
        public WindowsClientViewModel WindowsClientViewModel { get; }
        public ConnectionSettingsViewModel ConnectionSettingsViewModel { get; }
        public WindowsServiceViewModel WindowsServiceViewModel { get; }
        #endregion

        #region Constructor
        public MainWindowViewModel()
        {
            WindowsClientViewModel = new WindowsClientViewModel(new WindowsClientSettingsModel());
            ConnectionSettingsViewModel = new ConnectionSettingsViewModel();
            WindowsServiceViewModel = new WindowsServiceViewModel();
        }
        #endregion
    }
}