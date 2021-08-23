using asagiv.datapush.common.Interfaces;
using asagiv.datapush.ui.common.ViewModels;
using asagiv.datapush.ui.mobile.Models;
using asagiv.datapush.ui.mobile.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace asagiv.datapush.ui.mobile.ViewModels
{
    public class ClientSettingsViewModel : ClientSettingsViewModelBase
    {
        #region Constructor
        public ClientSettingsViewModel() : base(XFormsDataPushDbContext.Instance, new ClientSettingsModel()) { }
        public ClientSettingsViewModel(ClientSettingsModel clientModel) : base(XFormsDataPushDbContext.Instance, clientModel) { }
        #endregion

        #region Methods
        protected override async ValueTask UploadFilesAsync()
        {
            var files = await FilePicker.PickMultipleAsync();

            if (files == null || !files.Any())
            {
                return;
            }

            var fileNames = files.Select(x => x.FullPath);

            await UploadFilesAsync(fileNames);
        }

        public async Task PushShareStreamContexts(IEnumerable<ShareStreamContext> shareStreamContexts)
        {
            foreach(var streamContext in shareStreamContexts)
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
