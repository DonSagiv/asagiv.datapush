using asagiv.datapush.common.Utilities;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace asagiv.datapush.ui.Models
{
    public class DataPushClientModel : BindableBase
    {
        #region Fields
        private string _connectionString;
        private GrpcClient _client;
        private string _nodeName;
        #endregion

        #region Properties
        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; RaisePropertyChanged(nameof(ConnectionString)); }
        }
        public string NodeName
        {
            get { return _nodeName; }
            set { _nodeName = value; RaisePropertyChanged(nameof(NodeName)); }
        }
        #endregion

        #region Methods
        public async Task InitializeClientAsync()
        {
            
        }
        #endregion
    }
}
