using asagiv.pushrocket.common.Models;
using Grpc.Net.Client;
using ReactiveUI;
using Serilog;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;

namespace asagiv.pushrocket.ui.common.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        #region Fields
        private readonly ILogger _logger;
        private readonly Subject<string> _errorSubject = new();
        private string _connectionString;
        #endregion

        #region Properties
        public string ConnectionString
        {
            get => _connectionString;
            set => this.RaiseAndSetIfChanged(ref _connectionString, value);
        }
        public IObservable<string> ErrorObservable => _errorSubject.AsObservable();
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
            if (string.IsNullOrWhiteSpace(ConnectionString))
            {
                _errorSubject?.OnNext("Please enter a connection string.");

                return;
            }

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
                var message = $"Unable to connect to {ConnectionString}";

                _logger.Error(ex, message, ConnectionString);

                _errorSubject.OnNext(message);
            }
        }
        #endregion
    }
}
