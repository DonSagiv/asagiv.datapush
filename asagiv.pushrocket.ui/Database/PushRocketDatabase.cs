using asagiv.pushrocket.common.Models;
using Serilog;
using SQLite;

namespace asagiv.pushrocket.ui.Database
{
    public class PushRocketDatabase
    {
        #region Fields
        private readonly string _dbFileLocation = Path.Combine(FileSystem.Current.AppDataDirectory, "Data");
        private readonly ILogger _logger;
        private SQLiteAsyncConnection _connection;
        #endregion

        #region Properties
        public bool IsConnected => _connection is not null;
        #endregion

        #region Constructor
        public PushRocketDatabase(ILogger logger)
        {
            if (!Directory.Exists(_dbFileLocation))
            {
                Directory.CreateDirectory(_dbFileLocation);
            }

            _logger = logger;
        }
        #endregion

        public async Task ConnectAsync()
        {
            if (IsConnected)
            {
                return;
            }

            try
            {
                _connection = new SQLiteAsyncConnection(Path.Combine(_dbFileLocation, "Data.db"), SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.ReadWrite);

                await _connection.CreateTableAsync<ClientConnectionSettings>();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error connecting to databased");
            }
        }

        public async Task<IEnumerable<ClientConnectionSettings>> GetAllConnectionSettingsAsync()
        {
            return await _connection.Table<ClientConnectionSettings>()
                .ToArrayAsync();
        }

        public async Task AppendConnectionSettingAsync(ClientConnectionSettings settings)
        {
            await _connection.InsertOrReplaceAsync(settings);
        }

        public async Task DeleteConnectionSettingAsync(ClientConnectionSettings settings)
        {
            await _connection.DeleteAsync(settings);
        }
    }
}
