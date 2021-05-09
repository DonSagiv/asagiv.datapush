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
        #endregion

        #region Constructor
        public DataPushClientModel() { }
        #endregion

        #region Methods
        public bool initializeClient()
        {
            try
            {
                _client = new GrpcClient(_connectionString);
                _client.CreatePullSubscriber("Test");
                _client.DataRetrieved += async (s, e) => await OnDataRetrievedAsync(s, e);

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public async Task UploadFileAsync(string topic, string filePath)
        {
            await _client.PushFileAsync(topic, filePath);
        }

        private async Task OnDataRetrievedAsync(object sender, byte[] e)
        {
            var fileLocation = Path.Combine(@"C:\Users\DonSa\Desktop", "testFile.cs");

            using(var fs = new FileStream(fileLocation, FileMode.CreateNew))
            {
                await fs.WriteAsync(e);
            }
        }
        #endregion
    }
}
