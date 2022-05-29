using asagiv.pushrocket.common.Models;
using System.Threading.Tasks;

namespace asagiv.pushrocket.common.Interfaces
{
    public interface IPullNodeSettingsModel
    {
        #region Properties
        string NodeName { get; }
        string ConnectionString { get; }
        string DownloadLocation { get; }
        PullNodeStatus Status { get; }
        #endregion

        #region Methods
        Task UpdateServiceSettingsAsync();
        void InitializeService();
        void StopService();
        Task GetServiceStatusAsync();
        #endregion
    }
}
