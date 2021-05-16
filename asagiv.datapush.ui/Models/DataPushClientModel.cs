using asagiv.datapush.common;
using asagiv.datapush.common.Utilities;
using Prism.Mvvm;
using System;
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
        public async Task<bool> initializeClientAsync()
        {
            try
            {
                _client = new GrpcClient(_connectionString, NodeName, getDeviceId());
                await _client.CreatePullSubscriberAsync();
                _client.DataRetrieved += async (s, e) => await OnDataRetrievedAsync(s, e);

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        private string getDeviceId()
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

        private async Task OnDataRetrievedAsync(object _, DataPullResponse e)
        {
            var fileLocation = Path.Combine(SaveDirectory, e.Name);

            using var fs = new FileStream(fileLocation, FileMode.CreateNew);

            await fs.WriteAsync(e.Payload.ToByteArray());
        }
        #endregion
    }
}
