using asagiv.datapush.ui.mobile.ViewModels;
using System;
using System.Threading.Tasks;
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

            ClientSettingsView.BindingContext = ViewModel.DataPushViewModel;
            ConnectionSettingsView.BindingContext = ViewModel.ConnectionSettingsViewModel;

            Appearing += async (s, e) => await OnLoadedAsync(s, e);
            CurrentPageChanged += async(s,e) => await OnTabChangedAsync(s,e);
        }

        private Task OnLoadedAsync(object s, EventArgs e)
        {
            return ClientSettingsView.ViewModel.RefreshConnectionSettingsAsync();
        }

        private Task OnTabChangedAsync(object sender, EventArgs e)
        {
            if (CurrentPage == ClientSettingsView)
            {
                return ClientSettingsView.ViewModel.RefreshConnectionSettingsAsync();
            }
            else if (CurrentPage == ConnectionSettingsView)
            {
                return ConnectionSettingsView.ViewModel.RefreshConnectionSettingsAsync();
            }

            else return Task.CompletedTask;
        }
        #endregion
    }
}