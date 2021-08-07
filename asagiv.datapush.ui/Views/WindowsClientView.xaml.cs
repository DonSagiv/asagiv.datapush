using asagiv.datapush.ui.ViewModels;
using System.Windows.Controls;

namespace asagiv.datapush.ui.Views
{
    public partial class WindowsClientView : UserControl
    {
        #region Properties
        public WindowsClientViewModel ViewModel { get; private set; }
        #endregion

        #region Constructor
        public WindowsClientView()
        {
            InitializeComponent();
        }
        #endregion

        #region Methods
        public void SetViewModel(WindowsClientViewModel viewModelInput)
        {
            ViewModel = viewModelInput;
            DataContext = ViewModel;
        }
        #endregion
    }
}
