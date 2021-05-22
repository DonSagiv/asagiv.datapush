using asagiv.datapush.ui.Models;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace asagiv.datapush.ui.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        #region Fields
        private string _status;
        private ObservableCollection<string> _destinatioNodes;
        private string _selectedDestinationNode;
        #endregion

        #region Properties
        public DataPushClientModel ClientModel { get; }
        public string Status
        {
            get { return _status; }
            set { _status = value; RaisePropertyChanged(nameof(Status)); }
        }
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
        #endregion

        #region Commands
        public ICommand ConnectToServerCommand { get; }
        public ICommand SelectFileToUploadCommand { get; }
        public ICommand BrowseSaveLocationCommand { get; }
        #endregion

        #region Constructor
        public MainWindowViewModel()
        {
            ClientModel = new DataPushClientModel
            {
                // Default Value;
                ConnectionString = "http://localhost:80"
            };

            DestinationNodes = new ObservableCollection<string>();

            ConnectToServerCommand = new DelegateCommand(async() => await ConnectToServerAsync());
            SelectFileToUploadCommand = new DelegateCommand(async() => await UploadFileAsync());
            BrowseSaveLocationCommand = new DelegateCommand(BrowseSaveLocation);
        }
        #endregion

        #region Methods
        private async Task ConnectToServerAsync()
        {
            DestinationNodes.Clear();

            var nodes = await ClientModel.InitializeClientAsync();

            if(nodes == null)
            {
                Status = "Connection Failed";

                return;
            }

            foreach(var node in nodes)
            {
                Status = "Connection Successful";

                DestinationNodes.Add(node);
            }
        }

        private async Task UploadFileAsync()
        {
            var openFileDialog = new OpenFileDialog()
            {
                Title = "Select File to Upload.",
                CheckPathExists = true,
            };

            var result = openFileDialog.ShowDialog();

            if (result != DialogResult.OK || string.IsNullOrEmpty(SelectedDestinationNode))
            {
                return;
            }

            await ClientModel.PushFileAsync(SelectedDestinationNode, openFileDialog.FileName);
        }

        private void BrowseSaveLocation()
        {
            var dialog = new FolderBrowserDialog
            {
                SelectedPath = ClientModel?.SaveDirectory
            };

            var result = dialog.ShowDialog();

            if(result != DialogResult.OK)
            {
                return;
            }

            ClientModel.SaveDirectory = dialog.SelectedPath;
        }
        #endregion
    }
}