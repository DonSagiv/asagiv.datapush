using asagiv.datapush.ui.ViewModels;
using System.Windows.Controls;

namespace asagiv.datapush.ui.Views
{
    public partial class WindowsServiceView : UserControl
    {
        #region Properties
        public WindowsServiceViewModel ViewModel { get; private set; }
        #endregion

        #region Constructor
        public WindowsServiceView()
        {
            InitializeComponent();
        }
        #endregion

        #region Methods
        public void setViewModel(WindowsServiceViewModel viewModelInput)
        {
            ViewModel = viewModelInput;
            DataContext = ViewModel;
        }
        #endregion
    }
}
