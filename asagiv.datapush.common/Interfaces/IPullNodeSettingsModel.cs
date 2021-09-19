﻿using asagiv.datapush.common.Models;
using System.Threading.Tasks;

namespace asagiv.datapush.common.Interfaces
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
