using asagiv.datapush.common.Models;
using Microsoft.EntityFrameworkCore;

namespace asagiv.datapush.common.Utilities
{
    public abstract class DataPushDbContextBase : DbContext
    {
        public DbSet<ClientConnectionSettings> ConnectionSettingsSet { get; set; }
    }
}
