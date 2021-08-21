using asagiv.datapush.common.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using Xamarin.Essentials;

namespace asagiv.datapush.ui.mobile.Utilities
{
    public class XFormsDataPusDbContext : DataPushDbContextBase
    {
        #region Statics
        private readonly static Lazy<XFormsDataPusDbContext> _lazyInstance = new Lazy<XFormsDataPusDbContext>(() => new XFormsDataPusDbContext());
        public static XFormsDataPusDbContext Instance => _lazyInstance.Value;
        #endregion

        #region Constructor
        public XFormsDataPusDbContext()
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
