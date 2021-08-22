using ReactiveUI;

namespace asagiv.datapush.ui.mobile.ViewModels
{
    public class MainPageViewModel : ReactiveObject
    {
        public DataPushViewModel DataPushViewModel { get; }
        public ConnectionSettingsViewModel ConnectionSettingsViewModel { get; }

        public MainPageViewModel()
        {
            var dataPustViewModelObject = App.ServiceProvider.GetService(typeof(DataPushViewModel));
            var connectionSettingsViewModelObject = App.ServiceProvider.GetService(typeof(ConnectionSettingsViewModel));

            DataPushViewModel = dataPustViewModelObject as DataPushViewModel;
            ConnectionSettingsViewModel = connectionSettingsViewModelObject as ConnectionSettingsViewModel;
        }
    }
}
