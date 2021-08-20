using asagiv.datapush.ui.Models;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace asagiv.datapush.ui.ViewModels
{
    public class WindowsServiceViewModel : ReactiveObject
    {
        #region Fields
        private WinServiceStatus _status;
        #endregion

        #region Properties
        public WindowsServiceSettingsModel ServiceModel { get; }
        public WinServiceStatus Status
        {
            get { return _status; }
            set { this.RaiseAndSetIfChanged(ref _status,  value); }
        }
        #endregion

        #region Commands
        public ICommand StartServiceCommand { get; }
        public ICommand StopServiceCommand { get; }
        public ICommand UpdateSettingsCommand { get; }
        #endregion

        #region Constructor
        public WindowsServiceViewModel()
        {
            ServiceModel = new WindowsServiceSettingsModel();

            StartServiceCommand = ReactiveCommand.Create(async () => await StartServiceAsync());
            StopServiceCommand = ReactiveCommand.Create(async () => await StopServiceAsync());
            UpdateSettingsCommand = ReactiveCommand.Create(async () => await ServiceModel.UpdateServiceSettingsAsync());

            // Check the status of the service every second.
            _ = Observable.Interval(TimeSpan.FromSeconds(1))
                .Subscribe(async _ => await GetServiceStatusAsync());
        }
        #endregion

        #region methods
        private async Task StopServiceAsync()
        {
            await WindowsServiceSettingsModel.StopClientAsync();

            await GetServiceStatusAsync();
        }

        private async Task StartServiceAsync()
        {
            await WindowsServiceSettingsModel.InitializeClientAsync();

            await GetServiceStatusAsync();
        }

        private async Task GetServiceStatusAsync()
        {
            Status = await WindowsServiceSettingsModel.GetServiceStatus();
        }
        #endregion
    }
}
