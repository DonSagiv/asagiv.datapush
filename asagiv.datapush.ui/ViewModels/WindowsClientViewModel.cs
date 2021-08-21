﻿using asagiv.datapush.common.Interfaces;
using asagiv.datapush.ui.Models;
using asagiv.datapush.ui.Utilities;
using DynamicData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System;

namespace asagiv.datapush.ui.ViewModels
{
    public class WindowsClientViewModel : ReactiveObject
    {
        #region Properties
        public ObservableCollection<IClientConnectionSettings> ConnectionSettingsList { get; }
        public ObservableCollection<IDataPushContext> PushContextList { get; }
        public ObservableCollection<string> PullNodes { get; }
        public WindowsClientSettingsModel ClientModel { get; }
        #endregion

        #region Commands
        public ICommand SelectFileToUploadCommand { get; }
        #endregion

        #region Constructor
        public WindowsClientViewModel()
        {
            ClientModel = new WindowsClientSettingsModel();

            ConnectionSettingsList = new ObservableCollection<IClientConnectionSettings>();

            PullNodes = new ObservableCollection<string>();

            PushContextList = new ObservableCollection<IDataPushContext>();

            SelectFileToUploadCommand = ReactiveCommand.Create(async () => await UploadFilesAsync());

            this.WhenAnyValue(x => x.ClientModel.ConnectionSettings)
                .Where(x => x is not null)
                .Subscribe(async x => await ConnectClientAsync());
        }
        #endregion

        #region Methods
        public async Task RefreshConnectionSettingsAsync()
        {
            var selectedConnnectionSetting = ClientModel.ConnectionSettings?.Id;
            var selectedDestinationNode = ClientModel.DestinationNode;

            ConnectionSettingsList.Clear();

            var retrievedConnectionSettings = await WinUiDataPushDbContext.Instance.ConnectionSettingsSet
                .OrderBy(x => x.ConnectionName)
                .ToListAsync();

            ConnectionSettingsList.AddRange(retrievedConnectionSettings);

            ClientModel.ConnectionSettings = retrievedConnectionSettings.FirstOrDefault(x => x.Id == selectedConnnectionSetting);
            ClientModel.DestinationNode = selectedDestinationNode;
        }

        private async Task<bool> ConnectClientAsync()
        {
            PullNodes.Clear();

            var pullNodesToAdd = await ClientModel.ConnectClientAsync();

            PullNodes.AddRange(pullNodesToAdd);

            return true;
        }

        private async Task UploadFilesAsync()
        {
            var openFileDialog = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = true,
            };

            var result = openFileDialog.ShowDialog();

            if (result != true)
            {
                return;
            }

            foreach (var file in openFileDialog.FileNames)
            {
                var context = await ClientModel.CreatePushContextAsync(file);

                PushContextList.Add(context);

                await context.PushDataAsync();
            }
        }
        #endregion
    }
}
