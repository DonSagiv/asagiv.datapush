using asagiv.datapush.ui.Models;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System.Threading.Tasks;
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
        #endregion

        #region Constructor
        public MainWindowViewModel()
        {
            ClientModel = new DataPushClientModel();

            // Default Value;
            ClientModel.ConnectionString = "http://localhost:80";

            ConnectToServerCommand = new DelegateCommand(ConnectToServer);
            SelectFileToUploadCommand = new DelegateCommand(async() => await SelectFileToUpload());
        }
        #endregion

        #region Methods
        private void ConnectToServer()
        {
            Status = ClientModel.initializeClient()
                ? "Server Connection Successful"
                : "Server Connection Failed";
        }

        private async Task SelectFileToUpload()
        {
            var openFileDialog = new OpenFileDialog()
            {
                Title = "Select File to Upload.",
                CheckPathExists = true,
            };

            var result = openFileDialog.ShowDialog();

            if (result != true)
            {
                return;
            }

            await ClientModel.UploadFileAsync("Test", openFileDialog.FileName);
        }
        #endregion
    }
}