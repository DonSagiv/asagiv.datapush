using asagiv.datapush.common.Utilities;
using Microsoft.EntityFrameworkCore;
using System;

namespace asagiv.datapush.ui.Utilities
{
    public class WinUiDataPushDbContext : DataPushDbContextBase
    {
        #region Methods
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            optionsBuilder.UseSqlite($"Data Source={appDataFolder}//asagiv_datapush//DataPushDb.db;");
        }
        #endregion
    }
}
