using asagiv.datapush.common.Interfaces;
using asagiv.datapush.ui.common.ViewModels;
using asagiv.datapush.ui.mobile.Models;
using asagiv.datapush.ui.mobile.Utilities;
using Serilog;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using asagiv.datapush.ui.common.Interfaces;

namespace asagiv.datapush.ui.mobile.ViewModels
{
    public class ClientSettingsViewModel : ClientSettingsViewModelBase
    {
        #region Fields
        private IList<ShareStreamContext> _shareStreamContexts;
        #endregion

        #region Delegates
        public event EventHandler DestinatioNodeSelected;
        #endregion

        #region Properties
        public bool HasShareStreamContexts => _shareStreamContexts?.Any() ?? false;
        #endregion

        #region Commands
        public ICommand CancelShareCommand { get; }
        #endregion

        #region Constructor
        public ClientSettingsViewModel(XFormsDataPushDbContext dbContext, IClientSettingsModel model, ILogger logger) : base(dbContext, model, logger)
        {
            CancelShareCommand = ReactiveCommand.Create(ClearShareData);

            // Save the selected connection setting when selected.
            this.WhenAnyValue(x => x.ClientSettingsModel.ConnectionSettings)
                .Where(x => x != null)
                .Subscribe(x => Preferences.Set("Connection Setting", x.ConnectionName));

            // Save the selected destination node when selected.
            this.WhenAnyValue(x => x.ClientSettingsModel.DestinationNode)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Subscribe(x => Preferences.Set("Destination Node", x));
        }
        #endregion

        #region Methods
        public override async Task RefreshConnectionSettingsAsync()
        {
            await base.RefreshConnectionSettingsAsync();

            // Load the previously loaded settings.
            ClientSettingsModel.ConnectionSettings = ConnectionSettingsList
                .FirstOrDefault(x => x.ConnectionName == Preferences.Get("Connection Setting", null));
        }

        public async override Task ConnectToServerAsync()
        {
            await base.ConnectToServerAsync();

            // Remember the selected destination node from the last session.
            ClientSettingsModel.DestinationNode = DestinationNodes
                .FirstOrDefault(x => x == Preferences.Get("Destination Node", null));

            DestinatioNodeSelected?.Invoke(this, EventArgs.Empty);
        }

        public async override ValueTask UploadFilesAsync()
        {
            _logger?.Information("Attempting to Upload Files.");

            // Uploads the share stream context if there is one.
            if (HasShareStreamContexts)
            {
                await PushShareStreamContextsAsync();

                return;
            }

            // Loads the file picker, allows users to select files to upload.
            var files = await FilePicker.PickMultipleAsync();

            if (files == null || !files.Any())
            {
                return;
            }

            var fileNames = files.Select(x => x.FullPath);

            await UploadFilesAsync(fileNames);
        }

        public void PrepareShareStreamContexts(IEnumerable<ShareStreamContext> shareStreamContexts)
        {
            // Get share stream contexts (from "share" option). Set UI to "Share" mode.
            _shareStreamContexts = shareStreamContexts.ToList();

            this.RaisePropertyChanged(nameof(HasShareStreamContexts));
        }

        private void ClearShareData()
        {
            // Clear share stream contexts. Revert UI back to "Upload" mode.
            _shareStreamContexts = null;

            this.RaisePropertyChanged(nameof(HasShareStreamContexts));
        }

        private async Task PushShareStreamContextsAsync()
        {
            foreach (var streamContext in _shareStreamContexts)
            {
                var shareName = $"{streamContext.ShareFileName ?? Guid.NewGuid().ToString()}.{streamContext.Extension}";

                _logger.Information($"Creating Context for File: {shareName}.");

                // Stream from share stream to memory stream.
                using var ms = new MemoryStream();
                using (streamContext.InputStream)
                {
                    await streamContext.InputStream.CopyToAsync(ms);

                    var data = ms.ToArray();

                    await streamContext.InputStream.FlushAsync();

                    var pushContext = (ClientSettingsModel as ClientSettingsModel)?.CreatePushDataContext(shareName, data);

                    // Push data from memory stream.
                    await PushContextAsync(pushContext);
                }
            }

            ClearShareData();
        }

        protected async override Task PushContextAsync(IDataPushContext context)
        {
            _logger.Information($"Pushing Data for Context: {context.Name}.");

            try
            {
                // Insert the context at the beginning of the list.
                Device.BeginInvokeOnMainThread(() => 
                {
                    PushContextList.Insert(0, context);
                });

                // Push the data in the context to the server.
                await context.PushDataAsync();
            }
            catch (Exception e)
            {
                _logger.Error(e, e.Message);
            }

            _logger.Information($"Data Push Successful.");
        }
        #endregion
    }
}
