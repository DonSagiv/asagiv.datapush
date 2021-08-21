using asagiv.datapush.ui.mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace asagiv.datapush.ui.mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPageView : TabbedPage
    {
        #region ViewModels
        public MainPageViewModel ViewModel { get; }
        #endregion

        #region Constructor
        public MainPageView()
        {
            InitializeComponent();

            ViewModel = new MainPageViewModel();
            BindingContext = ViewModel;

            DataPushView.BindingContext = ViewModel.DataPushViewModel;

            Appearing += (s, e) => OnPageAppearingAsync(s, e);
        }

        private void OnPageAppearingAsync(object sender, System.EventArgs e)
        {
            ViewModel?.GetConnectionSettingsAsync();
        }
        #endregion
    }
}