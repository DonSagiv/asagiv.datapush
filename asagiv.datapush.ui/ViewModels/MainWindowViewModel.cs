using asagiv.datapush.ui.Models;
using Prism.Commands;
using Prism.Mvvm;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace asagiv.datapush.ui.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        #region Fields
        private string _status;
        #endregion

        #region Properties
        public DataPushClientModel ClientModel { get; }
        public string Status
        {
            get { return _status; }
            set { _status = value; RaisePropertyChanged(nameof(Status)); }
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

            ConnectToServerCommand = new DelegateCommand(async() => await ConnectToServerAsync());
            SelectFileToUploadCommand = new DelegateCommand(async() => await UploadFileAsync());
            BrowseSaveLocationCommand = new DelegateCommand(BrowseSaveLocation);
        }
        #endregion

        #region Methods
        private async Task ConnectToServerAsync()
        {
            Status = await ClientModel.initializeClientAsync()
                ? "Server Connection Successful"
                : "Server Connection Failed";
        }

        private async Task UploadFileAsync()
        {
            var openFileDialog = new OpenFileDialog()
            {
                Title = "Select File to Upload.",
                CheckPathExists = true,
            };

            var result = openFileDialog.ShowDialog();

            if (result != DialogResult.OK)
            {
                return;
            }

            await ClientModel.PushFileAsync("Test", openFileDialog.FileName);
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