using asagiv.datapush.ui.mobile.ViewModels;
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
        }
        #endregion
    }
}