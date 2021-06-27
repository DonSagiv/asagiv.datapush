using asagiv.datapush.ui.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace asagiv.datapush.ui.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        #region Fields
        private WinServiceStatus _status;
        private ObservableCollection<string> _destinatioNodes;
        private string _selectedDestinationNode;
        #endregion

        #region Properties
        public DataPushClientModel ClientModel { get; }
        public ObservableCollection<string> DestinationNodes
        {
            get { return _destinatioNodes; }
            set { _destinatioNodes = value; RaisePropertyChanged(nameof(DestinationNodes)); }
        }
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
        public ICommand SelectFileToUploadCommand { get; }
        public ICommand BrowseSaveLocationCommand { get; }
        #endregion

        #region Constructor
        public MainWindowViewModel()
        {
            ClientModel = new DataPushClientModel();

            DestinationNodes = new ObservableCollection<string>();

            StartServiceCommand = new DelegateCommand(async () => await StartServiceAsync());

            StopServiceCommand = new DelegateCommand(async () => await StopServiceAsync());

            UpdateSettingsCommand = new DelegateCommand(async () => await ClientModel.UpdateServiceSettingsAsync());

            // Check the status of the service every second.
            Observable.Interval(TimeSpan.FromSeconds(1))
                .Subscribe(async _ => await GetServiceStatusAsync());
        }

        private async Task StopServiceAsync()
        {
            await DataPushClientModel.StopClientAsync();

            await GetServiceStatusAsync();
        }

        private async Task StartServiceAsync()
        {
            await DataPushClientModel.InitializeClientAsync();

            await GetServiceStatusAsync();
        }

        private async Task GetServiceStatusAsync()
        {
            Status = await DataPushClientModel.GetServiceStatus();
        }
        #endregion
    }
}