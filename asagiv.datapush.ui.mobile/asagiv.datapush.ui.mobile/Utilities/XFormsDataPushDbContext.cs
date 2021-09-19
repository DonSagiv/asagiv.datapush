using asagiv.datapush.common.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using Xamarin.Essentials;

namespace asagiv.datapush.ui.mobile.Utilities
{
    public class XFormsDataPushDbContext : DataPushDbContextBase
    {
        #region Constructor
        public XFormsDataPushDbContext()
        {
            SQLitePCL.Batteries_V2.Init();

            Database.EnsureCreated();
        }
        #endregion

        #region Methods
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            optionsBuilder.UseSqlite($"Filename={FileSystem.AppDataDirectory}/DataPushDb.db3");
        }
        #endregion
    }
}
