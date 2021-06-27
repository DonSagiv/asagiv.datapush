using asagiv.datapush.ui.Models;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace asagiv.datapush.ui.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        #region Fields
        private WinServiceStatus _status;
        private string _selectedDestinationNode;
        #endregion

        #region Properties
        public WindowsServiceSettingsModel ServiceModel { get; }
        public WindowsClientSettingsModel ClientModel { get; }
        public string SelectedDestinationNode
        {
            get { return _selectedDestinationNode; }
            set { _selectedDestinationNode = value; RaisePropertyChanged(nameof(SelectedDestinationNode)); }
        }
        public WinServiceStatus Status
        {
            get { return _status; }
            set { _status = value; RaisePropertyChanged(nameof(Status)); }
        }
        #endregion

        #region Commands
        public ICommand StartServiceCommand { get; }
        public ICommand StopServiceCommand { get; }
        public ICommand UpdateSettingsCommand { get; }
        public ICommand ConnectClientCommand { get; }
        public ICommand SelectFileToUploadCommand { get; }
        #endregion

        #region Constructor
        public MainWindowViewModel()
        {
            ServiceModel = new WindowsServiceSettingsModel();
            ClientModel = new WindowsClientSettingsModel();

            StartServiceCommand = new DelegateCommand(async () => await StartServiceAsync());
            StopServiceCommand = new DelegateCommand(async () => await StopServiceAsync());
            UpdateSettingsCommand = new DelegateCommand(async () => await ServiceModel.UpdateServiceSettingsAsync());
            ConnectClientCommand = new DelegateCommand(async () => await ClientModel.ConnectClientAsync());
            SelectFileToUploadCommand = new DelegateCommand(async () => await UploadFilesAsync());

            // Check the status of the service every second.
            Observable.Interval(TimeSpan.FromSeconds(1))
                .Subscribe(async _ => await GetServiceStatusAsync());
        }

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

        private async Task UploadFilesAsync()
        {
            var openFileDialog = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = true,
            };

            var result = openFileDialog.ShowDialog();

            if(result != true)
            {
                return;
            }

            foreach(var file in openFileDialog.FileNames)
            {
                await ClientModel.PushFileAsync(SelectedDestinationNode, file);
            }
        }
        #endregion
    }
}