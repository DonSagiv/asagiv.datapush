using asagiv.datapush.ui.ViewModels;
using Serilog;
using System.Windows;

namespace asagiv.datapush.ui.Views
{
    public partial class MainWindow : Window
    {
        #region Fields
        private readonly ILogger _logger;
        #endregion

        #region Properties
        public MainWindowViewModel ViewModel { get; }
        #endregion

        public MainWindow(MainWindowViewModel mainWindowViewModel, ILogger logger)
        {
            _logger = logger;

            InitializeComponent();

            ViewModel = mainWindowViewModel;
            DataContext = ViewModel;

            WindowsClientView.SetViewModel(ViewModel.ClientSettingsViewModel);
            ConnectionSettingsView.SetViewModel(ViewModel.ConnectionSettingsViewModel);
            WindowsServiceView.SetViewModel(ViewModel.PullNodeSettingsViewModel);
        }
    }
}
