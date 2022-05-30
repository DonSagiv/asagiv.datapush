using asagiv.pushrocket.common.Models;
using Grpc.Net.Client;
using ReactiveUI;
using Serilog;
using System.Windows.Input;

namespace asagiv.pushrocket.ui.common.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        #region Fields
        private readonly ILogger _logger;
        private string _connectionString;
        #endregion

        #region Properties
        public string ConnectionString
        {
            get => _connectionString;
            set => this.RaiseAndSetIfChanged(ref _connectionString, value);
        }
        #endregion

        #region Commands
        public ICommand ConnectCommand { get; }
        #endregion

        #region Constructor
        public MainViewModel(ILogger logger)
        {
            _logger = logger;

            _logger?.Debug("Instantiating MainViewModel");

            ConnectCommand = ReactiveCommand.CreateFromTask(ConnectAsync);
        }
        #endregion

        #region Methods
        private async Task ConnectAsync()
        {
            var connectionSettings = new ClientConnectionSettings
            {
                ConnectionString = ConnectionString,
                ConnectionName = "TestConnection",
                NodeName = "Computer",
                IsPullNode = false,
            };

            _logger.Information("Appempting to connect to {ConnectionString}", ConnectionString);

            try
            {
                var channel = GrpcChannel.ForAddress(connectionSettings.ConnectionString);

                await channel.ConnectAsync();

                var connection = new GrpcClient(connectionSettings, channel, "Test");

                await connection.RegisterNodeAsync(false);

                _logger.Information("Successfully connected to {ConnectionString}", ConnectionString);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unable to connect to {ConnectionString}", ConnectionString);
            }
        }
        #endregion
    }
}
