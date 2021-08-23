using asagiv.datapush.ui.mobile.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace asagiv.datapush.ui.mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConnectionSettingsView : ContentPage
    {
        #region Properties
        public ConnectionSettingsViewModel ViewModel => BindingContext as ConnectionSettingsViewModel;
        #endregion

        #region Constructor
        public ConnectionSettingsView()
        {
            InitializeComponent();

            Appearing += async(s,e) => await OnPageLoading();
        }
        #endregion

        #region Methods
        private async ValueTask OnPageLoading()
        {
            if(ViewModel != null)
            {
                await ViewModel.RefreshConnectionSettingsAsync();
            }
        }
        #endregion
    }
}