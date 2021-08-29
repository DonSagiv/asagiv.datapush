using asagiv.datapush.ui.mobile.ViewModels;
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
            InitializeComponent();
        }
        #endregion
    }
}