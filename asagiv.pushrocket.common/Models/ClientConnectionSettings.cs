using asagiv.pushrocket.common.Interfaces;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace asagiv.pushrocket.common.Models
{
    public class ClientConnectionSettings : INotifyPropertyChanged, IClientConnectionSettings
    {
        #region Fields
        private uint _id;
        private string _connectionName;
        private string _connectionString;
        private string _nodeName;
        private bool _isPullNode;
        #endregion

        #region Delegates
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Properties
        [Key]
        public uint Id
        {
            get { return _id; }
            set { _id = value; RaisePropertyChanged(nameof(Id)); }
        }
        public string ConnectionName
        {
            get { return _connectionName; }
            set { _connectionName = value; RaisePropertyChanged(nameof(ConnectionName)); }
        }
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
        public bool IsPullNode
        {
            get { return _isPullNode; }
            set { _isPullNode = value; RaisePropertyChanged(nameof(IsPullNode)); }
        }
        #endregion

        #region Methods
        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
