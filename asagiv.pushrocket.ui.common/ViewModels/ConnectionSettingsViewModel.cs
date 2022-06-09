using asagiv.pushrocket.common.Models;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace asagiv.pushrocket.ui.common.ViewModels
{
    public class ConnectionSettingsViewModel : ReactiveObject
    {
        #region Fields
        private ClientConnectionSettings _selectedConnectionSettings;
        #endregion

        #region Properties
        public ObservableCollection<ClientConnectionSettings> ConnectionSettingsList { get; }
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
        public ConnectionSettingsViewModel()
        {
            ConnectionSettingsList = new();

            ConnectionSettingsList.CollectionChanged += (s, e) => this.RaisePropertyChanged();

            NewConnectionSettingsCommand = ReactiveCommand.Create(CreateNewConnection);
        }
        #endregion

        #region Methods
        private void CreateNewConnection() 
        {
            var connection = new ClientConnectionSettings();

            ConnectionSettingsList.Add(connection);

            SelectedConnectionSettings = connection;
        }
        #endregion
    }
}
