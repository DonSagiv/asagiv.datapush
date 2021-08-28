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

        public ClientSettingsView()
        {
            InitializeComponent();

            Appearing += async(s,e) => await OnPageAppearingAsync(s, e);
        }

        private async Task OnPageAppearingAsync(object sender, System.EventArgs e)
        {
            await ViewModel?.RefreshConnectionSettingsAsync();
        }
    }
}