using asagiv.common.Extensions;
using asagiv.pushrocket.common.Models;
using asagiv.pushrocket.ui.common.Database;
using asagiv.pushrocket.ui.common.Utilities;
using ReactiveUI;
using Serilog;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace asagiv.pushrocket.ui.common.ViewModels
{
    public class ConnectionSettingsViewModel : ReactiveObject
    {
        #region Fields
        private readonly WaitIndicatorService _waitIndicator;
        private readonly ILogger _logger;
        private readonly PushRocketDatabase _pushRocketDatabase;
        private string _selectedConnectionSettingString;
        private ClientConnectionSettings _selectedConnectionSettings;
        #endregion

        #region Properties
        public ObservableCollection<ClientConnectionSettings> ConnectionSettingsList { get; }
        public string SelectedConnectionSettingsString
        {
            get => _selectedConnectionSettingString;
            set => this.RaiseAndSetIfChanged(ref _selectedConnectionSettingString, value);
        }
        public ClientConnectionSettings SelectedConnectionSettings
        {
            get => _selectedConnectionSettings;
            set => this.RaiseAndSetIfChanged(ref _selectedConnectionSettings, value);
        }
        #endregion

        #region Commands
        public ICommand NewConnectionSettingsCommand { get; }
        public ICommand SaveConnectionSettingsCommand { get; }
        public ICommand DeleteConnectionSettingsCommand { get; }
        #endregion

        #region Constructor
        public ConnectionSettingsViewModel(WaitIndicatorService waitIndicator, PushRocketDatabase database, ILogger logger)
        {
            _waitIndicator = waitIndicator;
            _pushRocketDatabase = database;
            _logger = logger;

            _logger.Debug("Instantiating ConnectionSettingsViewModel");

            ConnectionSettingsList = new();

            ConnectionSettingsList.CollectionChanged += (s, e) => this.RaisePropertyChanged();

            this.WhenAnyValue(x => x.SelectedConnectionSettingsString)
                .Subscribe(OnSelectedConnectionSettingChanged);

            NewConnectionSettingsCommand = ReactiveCommand.Create(CreateNewConnection);
            SaveConnectionSettingsCommand = ReactiveCommand.CreateFromTask(SaveAllConnectionsAsync);
            DeleteConnectionSettingsCommand = ReactiveCommand.CreateFromTask(DeleteConnectionAsync);
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

            if (!connectionSettingsToAdd.Any())
            {
                CreateNewConnection();
            }
            else
            {
                ConnectionSettingsList.AddRange(connectionSettingsToAdd);
            }
        }

        private void OnSelectedConnectionSettingChanged(string inputConnectionName)
        {
            if(string.IsNullOrEmpty(inputConnectionName))
            {
                SelectedConnectionSettings = null;
            }

            SelectedConnectionSettings = ConnectionSettingsList
                .FirstOrDefault(x => x.ConnectionName == inputConnectionName);
        }

        private void CreateNewConnection()
        {
            _logger.Information("Creating New Connection Setting.");

            var connection = new ClientConnectionSettings
            {
                ConnectionName = "New Connection"
            };

            ConnectionSettingsList.Add(connection);

            SelectedConnectionSettings = connection;
        }

        private async Task SaveAllConnectionsAsync()
        {
            _logger.Information("Saving All Connection Settings.");

            foreach (var connection in ConnectionSettingsList)
            {
                await _pushRocketDatabase.AppendConnectionSettingAsync(connection);
            }
        }

        private async Task DeleteConnectionAsync()
        {
            if(SelectedConnectionSettings == null)
            {
                return;
            }

            _logger.Information("Deleting Connection Setting: {ConnectionSettingName}.", SelectedConnectionSettings.ConnectionName);

            await _pushRocketDatabase.DeleteConnectionSettingAsync(SelectedConnectionSettings);

            ConnectionSettingsList.Remove(SelectedConnectionSettings);

            if (!ConnectionSettingsList.Any())
            {
                CreateNewConnection();
            }
            else
            {
                SelectedConnectionSettingsString = ConnectionSettingsList.FirstOrDefault().ConnectionName;
            }
        }
        #endregion
    }
}
