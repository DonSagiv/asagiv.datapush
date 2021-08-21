using asagiv.datapush.common.Interfaces;
using asagiv.datapush.common.Models;
using asagiv.datapush.common.Utilities;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace asagiv.datapush.ui.Models
{
    public class WindowsClientSettingsModel : ReactiveObject, IDisposable
    {
        #region Fields
        private IGrpcClient _client;
        private string _pullNode;
        private string _nodeName;
        private IClientConnectionSettings _connectionSettings;
        #endregion

        #region Properties
        public IClientConnectionSettings ConnectionSettings
        {
            get { return _connectionSettings; }
            set { this.RaiseAndSetIfChanged(ref _connectionSettings, value); }
        }
        public string NodeName
        {
            get { return _nodeName; }
            set { this.RaiseAndSetIfChanged(ref _nodeName, value); }
        }
        public string PullNode
        {
            get { return _pullNode; }
            set { this.RaiseAndSetIfChanged(ref _pullNode, value); }
        }
        #endregion

        #region Methods
        public async Task<IList<string>> ConnectClientAsync()
        {
            var pullNodes = new List<string>();

            if (!IsConnectionSettingSelected())
            {
                return pullNodes;
            }

            try
            {
                _client = new GrpcClient(ConnectionSettings, GrpcClientFactory.GetDeviceId());

                var pullNodesToAdd = await _client.RegisterNodeAsync(false);

                pullNodes.AddRange(pullNodesToAdd);
            }
            catch (Exception ex)
            {
                // TODO: Log Error.
            }

            return pullNodes;
        }

        public async Task<IDataPushContext> CreatePushContextAsync(string filePath)
        {
            if (!IsPullNodeSelected())
            {
                return null;
            }

            try
            {
                var contextToAdd = await _client.CreatePushFileContextAsync(PullNode, filePath);

                return contextToAdd;
            }
            catch (Exception ex)
            {
                // TODO: Log Error.
            }

            return null;
        }

        private bool IsPullNodeSelected()
        {
            if (string.IsNullOrWhiteSpace(PullNode))
            {
                MessageBox.Show("Please select a destination.",
                    "No destination selected.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return false;
            }

            return true;
        }

        private bool IsConnectionSettingSelected()
        {
            if (_connectionSettings == null)
            {
                MessageBox.Show("Please select a Connection Setting.",
                    "Connection Setting not selcted.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return false;
            }

            return true;
        }

        public void Dispose()
        {
            _client.Dispose();
        }
        #endregion
    }
}
