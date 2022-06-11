using asagiv.common.Extensions;
using asagiv.pushrocket.common.Interfaces;
using asagiv.pushrocket.common.Models;
using asagiv.pushrocket.ui.common.Database;
using asagiv.pushrocket.ui.common.Utilities;
using Grpc.Net.Client;
using ReactiveUI;
using Serilog;
using System.Collections.ObjectModel;
using System.Reactive;
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
        private readonly PushRocketDatabase _pushRocketDatabase;
        private readonly Subject<string> _errorSubject = new();
        private bool _isConnected;
        private IGrpcClient _grpcClient;
        private string _selectedDestinationNode;
        private string _selectedConnectionSettingString;
        private ClientConnectionSettings _selectedConnectionSetting;
        #endregion

        #region Properties
        public bool IsConnected
        {
            get => _isConnected;
            private set => this.RaiseAndSetIfChanged(ref _isConnected, value);
        }
        public ObservableCollection<ClientConnectionSettings> ConnectionSettingsList { get; }
        public string SelectedConnectionSettingString
        {
            get => _selectedConnectionSettingString;
            set => this.RaiseAndSetIfChanged(ref _selectedConnectionSettingString, value);
        }
        public ClientConnectionSettings SelectedConnectionSettings
        {
            get => _selectedConnectionSetting;
            set => this.RaiseAndSetIfChanged(ref _selectedConnectionSetting, value);
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
        public ICommand PushFilesCommand { get; }
        #endregion

        #region Constructor
        public MainViewModel(WaitIndicatorService waitIndicator, PushRocketDatabase database, ILogger logger)
        {
            _pushRocketDatabase = database;

            _logger = logger;

            _logger?.Debug("Instantiating MainViewModel");

            _waitIndicator = waitIndicator;

            ConnectionSettingsList = new ObservableCollection<ClientConnectionSettings>();
            DestinationNodes = new ObservableCollection<string>();
            PushContexts = new ObservableCollection<IDataPushContext>();

            PushFilesCommand = ReactiveCommand.CreateFromTask(PushFilesAsync);

            ConnectionSettingsList = new();

            ConnectionSettingsList.CollectionChanged += (s, e) => this.RaisePropertyChanged();

            SelectedDestinationNode = null;

            this.WhenAnyValue(x => x.SelectedConnectionSettingString)
                .Subscribe(OnSelectedConnectionSettingChanged);

            this.WhenAnyValue(x => x.SelectedConnectionSettings)
                .SelectMany(ConnectAsync)
                .Subscribe();

            this.WhenAnyValue(x => x.SelectedDestinationNode)
                .Subscribe(OnSelectedDestinationNodeChanged);
        }
        #endregion

        #region Methods
        public async Task InitializeAsync()
        {
            await _pushRocketDatabase.ConnectAsync();

            if (!_pushRocketDatabase.IsConnected)
            {
                _logger.Error("Unable to connect to PushRocket database.");
            }

            ConnectionSettingsList.Clear();

            var connectionSettingsToAdd = await _pushRocketDatabase.GetAllConnectionSettingsAsync();

            ConnectionSettingsList.AddRange(connectionSettingsToAdd);
        }

        private void OnSelectedConnectionSettingChanged(string connectionNameInput)
        {
            if (string.IsNullOrWhiteSpace(connectionNameInput))
            {
                SelectedConnectionSettings = null;
            }

            SelectedConnectionSettings = ConnectionSettingsList
                .FirstOrDefault(x => x.ConnectionName == connectionNameInput);
        }

        private async Task<Unit> ConnectAsync(ClientConnectionSettings connectionSettings)
        {
            if(connectionSettings == null)
            {
                return Unit.Default;
            }

            _waitIndicator.ShowWaitIndicator();

            _logger.Information("Appempting to connect to {ConnectionString}", connectionSettings.ConnectionString);

            try
            {
                var channel = GrpcChannel.ForAddress(connectionSettings.ConnectionString);

                var timeoutTask = Task.Delay(10000);
                var connectTask = channel.ConnectAsync();

                await Task.WhenAny(timeoutTask, connectTask);

                if (!connectTask.IsCompletedSuccessfully)
                {
                    throw new TimeoutException($"Connection to {connectionSettings.ConnectionString} timed out.");
                }

                _grpcClient = new GrpcClient(connectionSettings, channel, "Test");

                var pullNodesToAdd = await _grpcClient.RegisterNodeAsync(false);

                DestinationNodes.Clear();

                DestinationNodes.AddRange(pullNodesToAdd);

                _logger.Information("Destination nodes retrieved from server.");

                _logger.Information("Successfully connected to {ConnectionString}", connectionSettings.ConnectionString);

                IsConnected = true;
            }
            catch (Exception ex)
            {
                var message = $"Unable to connect to {connectionSettings.ConnectionString}";

                _logger.Error(ex, message, connectionSettings.ConnectionString);

                _errorSubject.OnNext(message);
            }
            finally
            {
                _waitIndicator.HideWaitIndicator();
            }

            return Unit.Default;
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
            if (string.IsNullOrEmpty(selectedDestinationNode))
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
