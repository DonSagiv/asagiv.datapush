﻿using asagiv.datapush.common.Interfaces;
using asagiv.datapush.common.Models;
using asagiv.datapush.common.Utilities;
using asagiv.datapush.ui.common.Models;
using DynamicData;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace asagiv.datapush.ui.common.ViewModels
{
    public abstract class ClientSettingsViewModelBase : ReactiveObject
    {
        #region Fields
        private DataPushDbContextBase _dataPushDbContext;
        protected readonly ILogger _logger;
        #endregion

        #region Delegates
        public event EventHandler<string> ErrorOccurred;
        #endregion

        #region Properties
        public ObservableCollection<IClientConnectionSettings> ConnectionSettingsList { get; }
        public ObservableCollection<IDataPushContext> PushContextList { get; }
        public ObservableCollection<string> DestinationNodes { get; }
        public ClientSettingsModelBase ClientSettingsModel { get; }
        public bool IsConnected { get; private set; }
        #endregion

        #region Commands
        public ICommand SelectFileToUploadCommand { get; }
        #endregion

        #region Constructor
        public ClientSettingsViewModelBase(DataPushDbContextBase dataPushDbContext, ClientSettingsModelBase clientModel, ILogger logger)
        {
            _logger = logger;

            _dataPushDbContext = dataPushDbContext;

            ClientSettingsModel = clientModel;

            ConnectionSettingsList = new ObservableCollection<IClientConnectionSettings>();

            DestinationNodes = new ObservableCollection<string>();

            PushContextList = new ObservableCollection<IDataPushContext>();

            SelectFileToUploadCommand = ReactiveCommand.Create(async () => await UploadFilesAsync());

            var task = this.WhenAnyValue(x => x.ClientSettingsModel.ConnectionSettings)
                .Where(x => x != null)
                .ForEachAsync(async x => await ConnectClientAsync());
        }
        #endregion

        #region Methods
        public virtual async Task RefreshConnectionSettingsAsync()
        {
            var selectedConnnectionSetting = ClientSettingsModel.ConnectionSettings?.Id;
            var selectedDestinationNode = ClientSettingsModel.DestinationNode;

            ConnectionSettingsList.Clear();

            var retrievedConnectionSettings = await _dataPushDbContext.ConnectionSettingsSet
                .OrderBy(x => x.ConnectionName)
                .ToListAsync();

            ConnectionSettingsList.AddRange(retrievedConnectionSettings);

            ClientSettingsModel.ConnectionSettings = retrievedConnectionSettings.FirstOrDefault(x => x.Id == selectedConnnectionSetting);
            ClientSettingsModel.DestinationNode = selectedDestinationNode;
        }

        public virtual async Task ConnectClientAsync()
        {
            try
            {
                // Clear all destination nodes.
                DestinationNodes.Clear();

                // Connect to the client, retrieve pull nodes.
                var pullNodesToAdd = await ClientSettingsModel.ConnectClientAsync();

                _logger.Information($"Pull nodes found: {string.Join(", ", pullNodesToAdd)}");

                // Add pull nodes to the list.
                DestinationNodes.AddRange(pullNodesToAdd);
            }
            catch(TimeoutException e) when (e.Message == GrpcClient.timeoutMessage)
            {
                // Raise exception if connection has timed out.
                RaiseConnectionTimeoutException();
            }
        }

        protected void RaiseConnectionTimeoutException()
        {
            var message = $"Unable to establish connection to {ClientSettingsModel.ConnectionSettings.ConnectionName}.";

            _logger?.Warning(message);

            // Invoke the event that an error has occurred.
            ErrorOccurred?.Invoke(this, message);
        }

        protected abstract ValueTask UploadFilesAsync();

        protected async Task UploadFilesAsync(IEnumerable<string> fileNames)
        {
            foreach (var file in fileNames)
            {
                await PushFileAsync(file);
            }
        }

        protected async Task PushFileAsync(string file)
        {
            _logger?.Information($"Uploading file: {file}");

            var context = await ClientSettingsModel.CreatePushContextAsync(file);

            await PushContextAsync(context);
        }

        protected virtual async Task PushContextAsync(IDataPushContext context)
        {
            _logger.Information($"Adding {context.Name} to context list.");

            // Insert the push context to the top of the list.
            PushContextList.Insert(0, context);

            _logger.Information($"Pushing {context.Name}.");

            // Push the data in the context to the server.
            await context.PushDataAsync();
        }
        #endregion
    }
}
