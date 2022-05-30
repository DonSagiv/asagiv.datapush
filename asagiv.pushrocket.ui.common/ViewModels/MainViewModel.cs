using asagiv.pushrocket.common.Models;
using asagiv.pushrocket.ui.common.Utilities;
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
        private readonly WaitIndicatorService _waitIndicator;
        private readonly Subject<string> _errorSubject = new();
        private string _connectionString;
        private bool _isConnected;
        #endregion

        #region Properties
        public string ConnectionString
        {
            get => _connectionString;
            set => this.RaiseAndSetIfChanged(ref _connectionString, value);
        }
        public bool IsConnected
        {
            get => _isConnected;
            private set => this.RaiseAndSetIfChanged(ref _isConnected, value);
        }
        public IObservable<string> ErrorObservable => _errorSubject.AsObservable();
        #endregion

        #region Commands
        public ICommand ConnectCommand { get; }
        #endregion

        #region Constructor
        public MainViewModel(WaitIndicatorService waitIndicator, ILogger logger)
        {
            _logger = logger;

            _logger?.Debug("Instantiating MainViewModel");

            _waitIndicator = waitIndicator;

            ConnectCommand = ReactiveCommand.CreateFromTask(ConnectAsync);
        }
        #endregion

        #region Methods
        private async Task ConnectAsync()
        {
            _waitIndicator.ShowWaitIndicator();

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

                IsConnected = true;
            }
            catch (Exception ex)
            {
                var message = $"Unable to connect to {ConnectionString}";

                _logger.Error(ex, message, ConnectionString);

                _errorSubject.OnNext(message);
            }

            _waitIndicator.HideWaitIndicator();
        }
        #endregion
    }
}
