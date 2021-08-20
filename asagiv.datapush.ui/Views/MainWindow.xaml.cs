using asagiv.datapush.ui.ViewModels;
using System.Threading.Tasks;
using System.Windows;

namespace asagiv.datapush.ui.Views
{
    public partial class MainWindow : Window
    {
        #region Properties
        public MainWindowViewModel ViewModel { get; }
        #endregion

        public MainWindow()
        {
            InitializeComponent();

            ViewModel = new MainWindowViewModel();
            DataContext = ViewModel;

            WindowsClientView.SetViewModel(ViewModel.WindowsClientViewModel);
            ConnectionSettingsView.SetViewModel(ViewModel.ConnectionSettingsViewModel);
            WindowsServiceView.SetViewModel(ViewModel.WindowsServiceViewModel);

            Loaded += async (s,e) => await OnWindowLoadedAsync(s,e);
        }

        private async Task OnWindowLoadedAsync(object sender, RoutedEventArgs e)
        {
            await ViewModel?.InitializeAsync();
        }
    }
}
