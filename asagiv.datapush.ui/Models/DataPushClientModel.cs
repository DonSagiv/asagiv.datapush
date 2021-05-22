using asagiv.datapush.common;
using asagiv.datapush.common.Utilities;
using Grpc.Core;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace asagiv.datapush.ui.Models
{
    public class DataPushClientModel : BindableBase
    {
        #region Fields
        private string _fileToUploadPath;
        private string _connectionString;
        private GrpcClient _client;
        private string _nodeName;
        private string _saveDirectory;
        #endregion

        #region Properties
        public string FileToUploadPath
        {
            get { return _fileToUploadPath; }
            set { _fileToUploadPath = value; RaisePropertyChanged(nameof(FileToUploadPath)); }
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
        public string SaveDirectory
        {
            get { return _saveDirectory; }
            set { _saveDirectory = value; RaisePropertyChanged(nameof(SaveDirectory)); }
        }
        #endregion

        #region Constructor
        public DataPushClientModel()
        {
            SaveDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }
        #endregion

        #region Methods
        public async Task<IEnumerable<string>> InitializeClientAsync()
        {
            try
            {
                _client = new GrpcClient(_connectionString, NodeName, GetDeviceId());
                var availableNodes = await _client.RegisterNodeAsync(true);
                await _client.CreatePullSubscriberAsync();
                _client.DataRetrieved += async (s, e) => await OnDataRetrievedAsync(s, e);

                return availableNodes;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        private string GetDeviceId()
        {
            var appDataRoot = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appDataFolder = Path.Combine(appDataRoot, "asagiv_datapush");
            var deviceIdFile = Path.Combine(appDataFolder,"deviceId.txt");

            if (File.Exists(deviceIdFile))
            {
                return File.ReadAllText(deviceIdFile);
            }
              
            Directory.CreateDirectory(appDataFolder);

            var deviceId = Guid.NewGuid().ToString();

            File.WriteAllText(deviceIdFile, deviceId);

            return deviceId;
        }

        public async Task PushFileAsync(string destination, string filePath)
        {
            await _client.PushFileAsync(destination, filePath);
        }

        private async Task OnDataRetrievedAsync(object _, ResponseStreamContext<DataPullResponse> e)
        {
            var tempFileLocation = Path.Combine(SaveDirectory, $"{Guid.NewGuid()}.meta");
            var actualFileLocation = Path.Combine(SaveDirectory, e.ResponseData.Name);

            try
            {
                using var fs = new FileStream(tempFileLocation, FileMode.CreateNew);
                {
                    while (await e.ResponseStream.MoveNext())
                    {
                        await fs.WriteAsync(e.ResponseStream.Current.Payload.ToByteArray());
                    }
                }

                await fs.DisposeAsync();

                var iteration = 0;

                // Ensures that downloaded files do not overwrite files w/ the sane name.
                while (File.Exists(actualFileLocation))
                {
                    var nameWithoutExtension = Path.GetFileNameWithoutExtension(e.ResponseData.Name);
                    var extension = Path.GetExtension(e.ResponseData.Name);

                    actualFileLocation = Path.Combine(SaveDirectory, $"{nameWithoutExtension} ({++iteration}).{extension}");
                }

                File.Move(tempFileLocation, actualFileLocation);
            }
            catch(Exception ex)
            {
                File.Delete(tempFileLocation);
            }
        }
        #endregion
    }
}
