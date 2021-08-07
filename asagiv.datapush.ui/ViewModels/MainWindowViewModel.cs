using Prism.Mvvm;

namespace asagiv.datapush.ui.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        #region ViewModels
        public WindowsClientViewModel WindowsClientViewModel { get; }
        public WindowsServiceViewModel WindowsServiceViewModel { get; }
        #endregion

        #region Constructor
        public MainWindowViewModel()
        {
            WindowsClientViewModel = new WindowsClientViewModel();
            WindowsServiceViewModel = new WindowsServiceViewModel();
        }
        #endregion
    }
}