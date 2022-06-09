using asagiv.common.Extensions;
using asagiv.pushrocket.common.Interfaces;
using asagiv.pushrocket.common.Models;
using asagiv.pushrocket.ui.common.Interfaces;
using asagiv.pushrocket.ui.common.Utilities;
using Grpc.Net.Client;
using ReactiveUI;
using Serilog;
using System.Collections.ObjectModel;
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
        private IGrpcClient _grpcClient;
        private string _selectedDestinationNode;
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
        public ObservableCollection<string> DestinationNodes { get; }
        public ObservableCollection<IDataPushContext> PushContexts { get; }
        public string SelectedDestinationNode
        {
            get => _selectedDestinationNode;
            set => this.RaiseAndSetIfChanged(ref _selectedDestinationNode, value);
        }
        public IObservable<string> ErrorObservable => _errorSubject.AsObservable();
        #endregion

        #region Commands
        public ICommand ConnectCommand { get; }
        public ICommand PushFilesCommand { get; }
        #endregion

        #region Constructor
        public MainViewModel(WaitIndicatorService waitIndicator, ILogger logger)
        {
            _logger = logger;

            _logger?.Debug("Instantiating MainViewModel");

            _waitIndicator = waitIndicator;

            DestinationNodes = new ObservableCollection<string>();
            PushContexts = new ObservableCollection<IDataPushContext>();

            ConnectCommand = ReactiveCommand.CreateFromTask(ConnectAsync);
            PushFilesCommand = ReactiveCommand.CreateFromTask(PushFilesAsync);

            SelectedDestinationNode = null;

            this.WhenAnyValue(x => x.SelectedDestinationNode)
                .Subscribe(OnSelectedDestinationNodeChanged);
        }
        #endregion

        #region Methods
        private async Task ConnectAsync()
        {
            _waitIndicator.ShowWaitIndicator();

            if (string.IsNullOrWhiteSpace(ConnectionString))
            {
                _errorSubject?.OnNext("Please enter a connection string.");

                _waitIndicator.HideWaitIndicator();

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

                var timeoutTask = Task.Delay(10000);
                var connectTask = channel.ConnectAsync();

                await Task.WhenAny(timeoutTask, connectTask);

                if (!connectTask.IsCompletedSuccessfully)
                {
                    throw new TimeoutException($"Connection to {ConnectionString} timed out.");
                }

                _grpcClient = new GrpcClient(connectionSettings, channel, "Test");

                var pullNodesToAdd = await _grpcClient.RegisterNodeAsync(false);

                DestinationNodes.Clear();

                // Default Value for destination nodes
                DestinationNodes.Add("(None)");

                DestinationNodes.AddRange(pullNodesToAdd);

                _logger.Information("Destination nodes retrieved from server.");

                _logger.Information("Successfully connected to {ConnectionString}", ConnectionString);

                IsConnected = true;
            }
            catch (Exception ex)
            {
                var message = $"Unable to connect to {ConnectionString}";

                _logger.Error(ex, message, ConnectionString);

                _errorSubject.OnNext(message);
            }
            finally
            {
                _waitIndicator.HideWaitIndicator();
            }
        }

        private async Task PushFilesAsync()
        {
            try
            {
                var pickOptions = new PickOptions();

                var fileResultList = await FilePicker.Default.PickMultipleAsync(pickOptions);

                if (fileResultList?.Any() != true)
                {
                    return;
                }

                var contextsToAdd = fileResultList
                    .Select(x => _grpcClient.CreatePushFileContextAsync(SelectedDestinationNode, x.FileName, x.OpenReadAsync()))
                    .ToAsync();

                await foreach (var context in contextsToAdd)
                {
                    PushContexts.Add(context);

                    await context.PushDataAsync();
                }
            }
            catch (Exception ex)
            {
                const string message = "Unable to push file(s)";

                _logger.Error(ex, message);

                _errorSubject?.OnNext(message);
            }
        }

        public void OnSelectedDestinationNodeChanged(string selectedDestinationNode)
        {
            if (string.IsNullOrEmpty(selectedDestinationNode) || selectedDestinationNode == "(None)")
            {
                _logger?.Warning("Destination node de-Selected.");
            }
            else
            {
                _logger?.Information($"Selected destionation node: {SelectedDestinationNode}");
            }
        }

        public void RemoveContext(IDataPushContext contextToRemove)
        {
            PushContexts.Remove(contextToRemove);
        }
        #endregion
    }
}
