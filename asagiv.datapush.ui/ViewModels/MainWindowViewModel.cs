using Prism.Mvvm;
using System.Threading.Tasks;

namespace asagiv.datapush.ui.ViewModels
{
    public class MainWindowViewModel : BindableBase
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
        public Task InitializeAsync()
        {
            return ConnectionSettingsViewModel.InitializeAsync();
        }
        #endregion
    }
}