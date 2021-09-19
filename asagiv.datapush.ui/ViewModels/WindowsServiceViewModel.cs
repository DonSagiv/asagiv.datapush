using asagiv.datapush.common.Interfaces;
using asagiv.datapush.ui.common.Interfaces;
using ReactiveUI;
using Serilog;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace asagiv.datapush.ui.ViewModels
{
    public class WindowsServiceViewModel : ReactiveObject, IPullNodeSettingsViewModel
    {
        #region Fields
        private readonly ILogger _logger;
        #endregion

        #region Properties
        public IPullNodeSettingsModel ServiceModel { get; }

        #endregion

        #region Commands
        public ICommand StartServiceCommand { get; }
        public ICommand StopServiceCommand { get; }
        public ICommand UpdateSettingsCommand { get; }
        #endregion

        #region Constructor
        public WindowsServiceViewModel(IPullNodeSettingsModel pullNodeSettingsModel, ILogger logger)
        {
            _logger = logger;

            ServiceModel = pullNodeSettingsModel;

            StartServiceCommand = ReactiveCommand.Create(ServiceModel.InitializeService);
            StopServiceCommand = ReactiveCommand.Create(ServiceModel.StopService);
            UpdateSettingsCommand = ReactiveCommand.Create(async () => await ServiceModel.UpdateServiceSettingsAsync());

            // Check the status of the service every second.
            _ = Observable.Interval(TimeSpan.FromSeconds(0.5))
                .Subscribe(async _ => await GetServiceStatusAsync());
        }
        #endregion

        #region methods
        private async Task GetServiceStatusAsync()
        {
            await ServiceModel.GetServiceStatusAsync();
        }
        #endregion
    }
}
