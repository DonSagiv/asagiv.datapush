using asagiv.datapush.common.Utilities;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace asagiv.datapush.ui.Models
{
    public class WindowsClientSettingsModel : BindableBase
    {
        #region Fields
        private GrpcClient _client;
        private string _nodeName;
        private string _connectionString;
        #endregion

        #region Properties
        public string NodeName
        {
            get { return _nodeName; }
            set { _nodeName = value; RaisePropertyChanged(nameof(NodeName)); }
        }
        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; RaisePropertyChanged(nameof(ConnectionString)); }
        }
        public ObservableCollection<string> PullNodes { get; }
        #endregion

        #region Constructor
        public WindowsClientSettingsModel()
        {
            ConnectionString = "http://192.168.4.4:8082";
            NodeName = "Windows PC";

            PullNodes = new ObservableCollection<string>();
        }
        #endregion

        #region Methods
        public async Task ConnectClientAsync()
        {
            PullNodes.Clear();

            try
            {
                _client = new GrpcClient(ConnectionString, NodeName, GrpcClientFactory.GetDeviceId());

                var pullNodesToAdd = await _client.RegisterNodeAsync(false);

                PullNodes.AddRange(pullNodesToAdd);
            }
            catch(Exception ex)
            {
                // TODO: Log Error.
            }
        }

        public async Task PushFileAsync(string destinationNode, string filePath)
        {
            try
            {
                await _client.PushFileAsync(destinationNode, filePath);
            }
            catch(Exception ex)
            {
                // TODO: Log Error.
            }
        }
        #endregion
    }
}
