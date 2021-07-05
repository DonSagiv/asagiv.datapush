using Prism.Mvvm;

namespace asagiv.datapush.ui.mobile.ViewModels
{
    public class MainPageViewModel : BindableBase
    {
        public DataPushViewModel DataPushViewModel { get; }

        public MainPageViewModel()
        {
            DataPushViewModel = new DataPushViewModel();
        }
    }
}
