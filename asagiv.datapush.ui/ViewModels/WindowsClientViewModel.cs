using asagiv.datapush.ui.common.Models;
using asagiv.datapush.ui.common.ViewModels;
using asagiv.datapush.ui.Utilities;
using Microsoft.Win32;
using System.Threading.Tasks;

namespace asagiv.datapush.ui.ViewModels
{
    public class WindowsClientViewModel : ClientSettingsViewModelBase
    {
        #region Constructor
        public WindowsClientViewModel(ClientSettingsModelBase clientSettingsModel) : base(WinUiDataPushDbContext.Instance, clientSettingsModel) { }
        #endregion

        #region Methods
        protected override async ValueTask UploadFilesAsync()
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

            await UploadFilesAsync(openFileDialog.FileNames);
        }
        #endregion
    }
}
