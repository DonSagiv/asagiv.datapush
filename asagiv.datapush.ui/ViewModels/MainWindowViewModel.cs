using ReactiveUI;
using System.Threading.Tasks;

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
            WindowsClientViewModel = new WindowsClientViewModel();
            ConnectionSettingsViewModel = new ConnectionSettingsViewModel();
            WindowsServiceViewModel = new WindowsServiceViewModel();
        }
        #endregion

        #region Methods
        public async Task InitializeAsync()
        {
            await WindowsClientViewModel.InitializeAsync();
            await ConnectionSettingsViewModel.InitializeAsync();
        }
        #endregion
    }
}