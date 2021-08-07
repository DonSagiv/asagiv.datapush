using System;

namespace asagiv.datapush.common.Models
{
    public class ClientConnectionSettings
    {
        #region Properties
        public string ConnectionString { get; }
        public bool IsPullNode { get; }
        #endregion

        #region Constructor
        public ClientConnectionSettings(string connectionString, bool isPullNode)
        {
            ConnectionString = connectionString;
            IsPullNode = isPullNode;
        }
        #endregion
    }
}
