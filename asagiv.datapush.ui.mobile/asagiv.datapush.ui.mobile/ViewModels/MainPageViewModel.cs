using asagiv.datapush.common.Utilities;
using Grpc.Core;
using Grpc.Net.Client;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;

namespace asagiv.datapush.ui.mobile.ViewModels
{
    public class MainPageViewModel : BindableBase
    {
        #region Fields
        private string _connectionString;
        #endregion

        #region Properties
        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; RaisePropertyChanged(nameof(ConnectionString)); }
        }
        public GrpcClient Client { get; private set; }
        #endregion

        #region Commands
        public ICommand ConnectCommand { get; }
        public ICommand SelectFileCommand { get; }
        #endregion

        #region Constructor
        public MainPageViewModel()
        {
            ConnectCommand = new DelegateCommand(Connect);
            SelectFileCommand = new DelegateCommand(async () => await SelectFileAsync());
        }
        #endregion

        #region Methods
        private void Connect()
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            var channel = new Channel(_connectionString, ChannelCredentials.Insecure);

            Client = new GrpcClient(channel);
        }

        private async Task SelectFileAsync()
        {
            var file = await FilePicker.PickAsync();

            var fileName = file.FullPath;

            var data = await File.ReadAllBytesAsync(fileName);

            await Client.PushDataAsync("Test", data);
        }
        #endregion
    }
}
