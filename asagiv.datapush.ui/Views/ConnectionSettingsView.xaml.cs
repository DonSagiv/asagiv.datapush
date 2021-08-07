using asagiv.datapush.ui.ViewModels;
using System.Windows.Controls;

namespace asagiv.datapush.ui.Views
{
    public partial class ConnectionSettingsView : UserControl
    {
        #region Properties
        public ConnectionSettingsViewModel ViewModel { get; private set; }
        #endregion

        #region Constructor
        public ConnectionSettingsView()
        {
            InitializeComponent();
        }
        #endregion

        #region Methods
        public void setViewModel(ConnectionSettingsViewModel viewModelInput)
        {
            ViewModel = viewModelInput;
            DataContext = ViewModel;
        }
        #endregion
    }
}
