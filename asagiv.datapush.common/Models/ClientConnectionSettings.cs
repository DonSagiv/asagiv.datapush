using System.ComponentModel.DataAnnotations;

namespace asagiv.datapush.common.Models
{
    public class ClientConnectionSettings
    {
        #region Properties
        [Key]
        public long Id { get; set; }
        public string ConnectionString { get; set; }
        public bool IsPullNode { get; set; }
        #endregion

        #region Constructor
        protected ClientConnectionSettings() { }
        public ClientConnectionSettings(string connectionString, bool isPullNode) : this()
        {
            ConnectionString = connectionString;
            IsPullNode = isPullNode;
        }
        #endregion
    }
}
