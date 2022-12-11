using asagiv.common.Extensions;
using asagiv.pushrocket.common.Interfaces;
using asagiv.pushrocket.common.Models;
using asagiv.pushrocket.ui.common.Database;
using asagiv.pushrocket.ui.common.Utilities;
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
        private readonly PushRocketDatabase _pushRocketDatabase;
        private readonly Subject<string> _errorSubject = new Subject<string>();
        private readonly IClientSettingsModel _clientSettingsModel;
        private bool _isConnected;
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
        public MainViewModel(WaitIndicatorService waitIndicator, PushRocketDatabase database, ILogger logger, IClientSettingsModel clientSettingsModel)
        {
            _pushRocketDatabase = database;

            _logger = logger;

            _logger?.Debug("Instantiating MainViewModel");

            _waitIndicator = waitIndicator;

            _clientSettingsModel = clientSettingsModel;

            ConnectionSettingsList = new();
            DestinationNodes = new();
            PushContexts = new();

            PushFilesCommand = ReactiveCommand.CreateFromTask(PushFilesAsync);

            ConnectionSettingsList.CollectionChanged += (s, e) => this.RaisePropertyChanged();

            SelectedDestinationNode = null;

            this.WhenAnyValue(x => x.SelectedConnectionSettingString)
                .Subscribe(OnSelectedConnectionSettingChanged);

            this.WhenAnyValue(x => x.SelectedConnectionSettings)
                .SelectMany(ConnectAsync)
                .Subscribe(OnConnected);

            this.WhenAnyValue(x => x.SelectedDestinationNode)
                .Subscribe(OnSelectedDestinationNodeChanged);
        }
        #endregion

        #region Methods
        public async Task InitializeAsync()
        {
            // Connect to the SqlLite database.
            await _pushRocketDatabase.ConnectAsync();

            if (!_pushRocketDatabase.IsConnected)
            {
                _logger.Error("Unable to connect to PushRocket database.");
            }

            ConnectionSettingsList.Clear();

            // Get all the connection settings from the SqlLite database.
            var connectionSettingsToAdd = await _pushRocketDatabase.GetAllConnectionSettingsAsync();

            ConnectionSettingsList.AddRange(connectionSettingsToAdd);

            try
            {
                // Get the last-used connection setting.
                var lastConnectionSettingIdString = Preferences.Get("LastConnectedSettingsId", String.Empty);

                if(!uint.TryParse(lastConnectionSettingIdString, out var lastConnectionSettingId))
                {
                    return;
                }

                var lastConnectionSetting = ConnectionSettingsList
                    .FirstOrDefault(x => x.Id == lastConnectionSettingId);

                if (lastConnectionSetting is null)
                {
                    return;
                }

                SelectedConnectionSettingString = lastConnectionSetting.ConnectionName;
            }
            catch(Exception ex)
            {
                _logger.Error(ex, "Connection Load Error");
            }
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

        private async Task<ClientConnectionSettings> ConnectAsync(ClientConnectionSettings connectionSettings)
        {
            // If no connection settings are selected, do nothing.
            if(connectionSettings == null)
            {
                return null;
            }

            _waitIndicator.ShowWaitIndicator();

            // Create a new client settings model w/ the connection settings.
            _clientSettingsModel.ConnectionSettings = _selectedConnectionSetting;

            try
            {
                // Attempt to connect to the server, get all destination nodes.
                var destinationNodes = await _clientSettingsModel.ConnectToServerAsync();

                // Add the destination nodes to the list.
                DestinationNodes.AddRange(destinationNodes);

                // Get the last-used destination node, set it as the current destination node.
                var previousDestinationNode = Preferences.Get("LastDestinationNode", string.Empty);

                SelectedDestinationNode = DestinationNodes.FirstOrDefault(x => x == previousDestinationNode);

                return connectionSettings;
            }
            catch (Exception ex)
            {
                var message = $"Unable to connect to {connectionSettings.ConnectionString}";

                _logger.Error(ex, message, connectionSettings.ConnectionString);

                _errorSubject?.OnNext(message);

                return null;
            }
            finally
            {
                _waitIndicator.HideWaitIndicator();
            }
        }

        private void OnConnected(ClientConnectionSettings connectionSettings)
        {
            if(connectionSettings == null)
            {
                IsConnected = false;

                return;
            }

            IsConnected = true;

            Preferences.Set("LastConnectedSettingsId", connectionSettings?.Id.ToString());
        }

        public void OnSelectedDestinationNodeChanged(string selectedDestinationNodeInput)
        {
            if (string.IsNullOrEmpty(selectedDestinationNodeInput))
            {
                _logger?.Warning("Destination node de-Selected.");
            }
            else
            {
                _logger?.Information($"Selected destionation node: {SelectedDestinationNode}");

                Preferences.Set("LastDestinationNode", selectedDestinationNodeInput);
            }
        }

        private async Task PushFilesAsync()
        {
            try
            {
                // Use the file picker to browser for the files to transfer.
                var pickOptions = new PickOptions();

                var fileResultList = await FilePicker.Default.PickMultipleAsync(pickOptions);

                if (fileResultList?.Any() != true)
                {
                    return;
                }

                // Create a context for pushing the each selected file.
                var contextsToAddTasks = fileResultList
                    .Select(x => _clientSettingsModel.Client.CreatePushFileContextAsync(SelectedDestinationNode, x.FileName, x.OpenReadAsync()));

                var contextsToAdd = await Task.WhenAll(contextsToAddTasks);

                PushContexts.AddRange(contextsToAdd);

                // Start with the last selected file so that it appears in the correct order in the UI.
                foreach (var context in contextsToAdd.Reverse())
                {
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

        public void RemoveContext(IDataPushContext contextToRemove)
        {
            PushContexts.Remove(contextToRemove);
        }
        #endregion
    }
}
