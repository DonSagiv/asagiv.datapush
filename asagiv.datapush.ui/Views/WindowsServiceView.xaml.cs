using asagiv.datapush.ui.common.Interfaces;
using System.Windows.Controls;

namespace asagiv.datapush.ui.Views
{
    public partial class WindowsServiceView : UserControl
    {
        #region Properties
        public IPullNodeSettingsViewModel ViewModel { get; private set; }
        #endregion

        #region Constructor
        public WindowsServiceView()
        {
            InitializeComponent();
        }
        #endregion

        #region Methods
        public void SetViewModel(IPullNodeSettingsViewModel viewModelInput)
        {
            ViewModel = viewModelInput;
            DataContext = ViewModel;
        }
        #endregion
    }
}
