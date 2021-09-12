using asagiv.datapush.ui.common.Models;
using asagiv.datapush.ui.common.ViewModels;
using asagiv.datapush.ui.Models;
using asagiv.datapush.ui.Utilities;
using Microsoft.Win32;
using Serilog;
using System.Threading.Tasks;

namespace asagiv.datapush.ui.ViewModels
{
    public class WindowsClientViewModel : ClientSettingsViewModelBase
    {
        #region Constructor
        public WindowsClientViewModel(WinUiDataPushDbContext dbContext, WindowsClientSettingsModel clientSettingsModel, ILogger logger) : base(dbContext, clientSettingsModel, logger) { }
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

            _logger.Information("Showing Upload Files Dialog.");

            var result = openFileDialog.ShowDialog();

            if (result != true)
            {
                _logger.Information("File selection cancelled.");

                return;
            }

            await UploadFilesAsync(openFileDialog.FileNames);
        }
        #endregion
    }
}
