using ReactiveUI;

namespace asagiv.datapush.ui.mobile.ViewModels
{
    public class MainPageViewModel : ReactiveObject
    {
        #region ViewModels
        public ClientSettingsViewModel DataPushViewModel { get; }
        public ConnectionSettingsViewModel ConnectionSettingsViewModel { get; }
        #endregion

        public MainPageViewModel()
        {
            var dataPustViewModelObject = App.ServiceProvider.GetService(typeof(ClientSettingsViewModel));
            var connectionSettingsViewModelObject = App.ServiceProvider.GetService(typeof(ConnectionSettingsViewModel));

            DataPushViewModel = dataPustViewModelObject as ClientSettingsViewModel;
            ConnectionSettingsViewModel = connectionSettingsViewModelObject as ConnectionSettingsViewModel;
        }
    }
}
