using asagiv.datapush.common.Interfaces;
using asagiv.datapush.ui.common.ViewModels;
using asagiv.datapush.ui.mobile.Models;
using asagiv.datapush.ui.mobile.Utilities;
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
        public ClientSettingsViewModel() : base(XFormsDataPushDbContext.Instance, new ClientSettingsModel()) 
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

        public async override Task<bool> ConnectClientAsync()
        {
            var isConnected = await base.ConnectClientAsync();

            if (isConnected)
            {
                ClientSettingsModel.DestinationNode = DestinationNodes
                    .FirstOrDefault(x => x == Preferences.Get("Destination Node", null));
            }

            DestinatioNodeSelected?.Invoke(this, EventArgs.Empty);

            return isConnected;
        }

        protected async override ValueTask UploadFilesAsync()
        {
            if (HasShareStreamContexts)
            {
                await PushShareStreamContextsAsync();

                return;
            }

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
            _shareStreamContexts = shareStreamContexts.ToList();

            this.RaisePropertyChanged(nameof(HasShareStreamContexts));
        }

        private void ClearShareData()
        {
            _shareStreamContexts = null;

            this.RaisePropertyChanged(nameof(HasShareStreamContexts));
        }

        private async Task PushShareStreamContextsAsync()
        {
            foreach(var streamContext in _shareStreamContexts)
            {
                var shareName = $"{streamContext.ShareFileName ?? Guid.NewGuid().ToString()}.{streamContext.Extension}";

                LoggerInstance.Instance.Log.Information($"Creating Context for File: {shareName}.");

                using var ms = new MemoryStream();
                using (streamContext.InputStream)
                {
                    await streamContext.InputStream.CopyToAsync(ms);

                    var data = ms.ToArray();

                    await streamContext.InputStream.FlushAsync();

                    var pushContext = (ClientSettingsModel as ClientSettingsModel)?.CreatePushDataContext(shareName, data);

                    await PushContextAsync(pushContext);
                }
            }

            ClearShareData();
        }

        protected override async Task PushContextAsync(IDataPushContext context)
        {
            LoggerInstance.Instance.Log.Information($"Pushing Data for Context: {context.Name}.");

            try
            {
                Device.BeginInvokeOnMainThread(() => 
                {
                    PushContextList.Insert(0, context);
                });
                
            }
            catch(Exception e)
            {
                LoggerInstance.Instance.Log.Error(e, e.Message);
            }

            await context.PushDataAsync();

            LoggerInstance.Instance.Log.Information($"Data Push Successful.");
        }
        #endregion
    }
}
