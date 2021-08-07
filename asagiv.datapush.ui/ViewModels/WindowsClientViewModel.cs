using asagiv.datapush.ui.Models;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System.Threading.Tasks;
using System.Windows.Input;

namespace asagiv.datapush.ui.ViewModels
{
    public class WindowsClientViewModel : BindableBase
    {
        #region Fields
        private string _selectedDestinationNode;
        #endregion

        #region Properties
        public WindowsClientSettingsModel ClientModel { get; }
        public string SelectedDestinationNode
        {
            get { return _selectedDestinationNode; }
            set { _selectedDestinationNode = value; RaisePropertyChanged(nameof(SelectedDestinationNode)); }
        }
        #endregion

        #region Commands
        public ICommand ConnectClientCommand { get; }
        public ICommand SelectFileToUploadCommand { get; }
        #endregion

        #region Constructor
        public WindowsClientViewModel()
        {
            ClientModel = new WindowsClientSettingsModel();

            ConnectClientCommand = new DelegateCommand(async () => await ClientModel.ConnectClientAsync());
            SelectFileToUploadCommand = new DelegateCommand(async () => await UploadFilesAsync());
        }
        #endregion

        #region Methods
        private async Task UploadFilesAsync()
        {
            var openFileDialog = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = true,
            };

            var result = openFileDialog.ShowDialog();

            if (result != true)
            {
                return;
            }

            foreach (var file in openFileDialog.FileNames)
            {
                await ClientModel.PushFileAsync(SelectedDestinationNode, file);
            }
        }
        #endregion
    }
}
