using asagiv.datapush.common.Utilities;
using Microsoft.EntityFrameworkCore;
using System;

namespace asagiv.datapush.ui.Utilities
{
    public class WinUiDataPushDbContext : DataPushDbContextBase
    {
        #region Statics
        private static Lazy<WinUiDataPushDbContext> _lazyInstance = new Lazy<WinUiDataPushDbContext>(() => new WinUiDataPushDbContext());
        #endregion

        #region Methods
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            optionsBuilder.UseSqlite($"Data Source={appDataFolder}//DataPushDb.db;");
        }
        #endregion
    }
}
