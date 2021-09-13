using asagiv.datapush.ui.common.Interfaces;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace asagiv.datapush.ui.Views
{
    public partial class WindowsClientView : UserControl
    {
        #region Properties
        public IClientSettingsViewModel ViewModel { get; private set; }
        #endregion

        #region Constructor
        public WindowsClientView()
        {
            InitializeComponent();

            IsVisibleChanged += async (s, e) => await OnVisibilityChanged();
        }
        #endregion

        #region Methods
        public void SetViewModel(IClientSettingsViewModel viewModelInput)
        {
            ViewModel = viewModelInput;
            DataContext = ViewModel;
        }

        private async ValueTask OnVisibilityChanged()
        {
            // Lazy load connection settings.

            if (!IsVisible)
            {
                // "Hot" path
                return; 
            }
            else
            {
                // "Cold" path
                await ViewModel.RefreshConnectionSettingsAsync();
            }
        }
        #endregion
    }
}
