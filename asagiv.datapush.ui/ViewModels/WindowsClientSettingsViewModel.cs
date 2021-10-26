using asagiv.datapush.common.Interfaces;
using asagiv.datapush.common.Models;
using asagiv.datapush.ui.common.Interfaces;
using asagiv.datapush.ui.common.ViewModels;
using asagiv.datapush.ui.Utilities;
using Microsoft.Win32;
using Serilog;
using System.Threading.Tasks;

namespace asagiv.datapush.ui.ViewModels
{
    public class WindowsClientSettingsViewModel : ClientSettingsViewModelBase
    {
        #region Constructor
        public WindowsClientSettingsViewModel(WinUiDataPushDbContext dbContext, IClientSettingsModel clientSettingsModel, ILogger logger) : base(dbContext, clientSettingsModel, logger) { }
        #endregion

        #region Methods
        public async override ValueTask UploadFilesAsync()
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

        public override IDataPushContextViewModel getDataContextViewModel(IDataPushContext dataPushContext)
        {
            return new DataPushContextViewModel(dataPushContext);
        }

        #endregion
    }
}
