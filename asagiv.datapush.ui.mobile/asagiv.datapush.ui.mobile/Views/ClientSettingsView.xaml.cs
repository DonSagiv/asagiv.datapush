using asagiv.datapush.ui.mobile.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace asagiv.datapush.ui.mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClientSettingsView : ContentPage
    {
        #region ViewModels
        public ClientSettingsViewModel ViewModel => BindingContext as ClientSettingsViewModel;
        #endregion

        #region Constructor
        public ClientSettingsView()
        {
            this.BindingContextChanged += ClientSettingsView_BindingContextChanged;

            InitializeComponent();
        }

        private void ClientSettingsView_BindingContextChanged(object sender, System.EventArgs e)
        {
            ViewModel.ErrorOccurred += async (s, e) => await OnErrorOccurred(s,e);
        }

        private async Task OnErrorOccurred(object sender, string e)
        {
            await DisplayAlert("Error Occurred", e, "OK");
        }
        #endregion
    }
}