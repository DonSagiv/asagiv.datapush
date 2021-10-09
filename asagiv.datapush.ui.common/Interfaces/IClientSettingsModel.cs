using asagiv.datapush.common.Interfaces;
using asagiv.datapush.common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace asagiv.datapush.ui.common.Interfaces
{
    public interface IClientSettingsModel : IDisposable
    {
        #region Properties
        IClientConnectionSettings ConnectionSettings { get; set; }
        string DestinationNode { get; set; }
        IGrpcClient Client { get; }
        #endregion

        #region Methods
        Task<IList<string>> ConnectToServerAsync();
        Task<DataPushContext> CreatePushContextAsync(string filePath);
        #endregion
    }
}
