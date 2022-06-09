using asagiv.pushrocket.common.Models;
using Serilog;
using SQLite;

namespace asagiv.pushrocket.ui.common.Database
{
    public class PushRocketDatabase
    {
        #region Fields
        private readonly string _dbFileLocation;
        private readonly ILogger _logger;
        #endregion

        #region Constructor
        public PushRocketDatabase(ILogger logger)
        {
            _dbFileLocation = Path.Combine(FileSystem.Current.AppDataDirectory, "Data");

            if (!Directory.Exists(_dbFileLocation))
            {
                Directory.CreateDirectory(_dbFileLocation);
            }

            _logger = logger;
        }
        #endregion

        public async Task ConnectAsync()
        {
            try
            {
                var conn = new SQLiteAsyncConnection(Path.Combine(_dbFileLocation, "Data.db"), SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.ReadWrite);

                await conn.CreateTableAsync<ClientConnectionSettings>();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error connecting to databased");
            }
        }
    }
}
