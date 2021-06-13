using asagiv.datapush.ui.mobile.ViewModels;
using Xamarin.Forms;

namespace asagiv.datapush.ui.mobile
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
        }
    }
}
