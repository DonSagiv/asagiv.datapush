using asagiv.datapush.ui.common.Interfaces;
using asagiv.datapush.ui.ViewModels;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace asagiv.datapush.ui.Views
{
    public partial class ConnectionSettingsView : UserControl
    {
        #region Properties
        public IConnectionSettingsViewModel ViewModel { get; private set; }
        #endregion

        #region Constructor
        public ConnectionSettingsView()
        {
            InitializeComponent();

            IsVisibleChanged += async(s,e) => await OnVisibiltyChangedAsync();
        }
        #endregion

        #region Methods
        public void SetViewModel(IConnectionSettingsViewModel viewModelInput)
        {
            ViewModel = viewModelInput;
            DataContext = ViewModel;
        }

        private async ValueTask OnVisibiltyChangedAsync()
        {
            if (!IsVisible)
            {
                return;
            }
            else
            {
                await ViewModel.RefreshConnectionSettingsAsync();
            }
        }
        #endregion
    }
}
