using asagiv.pushrocket.common.Models;
using Microsoft.EntityFrameworkCore;

namespace asagiv.pushrocket.common.Utilities
{
    public abstract class DataPushDbContextBase : DbContext
    {
        public DbSet<ClientConnectionSettings> ConnectionSettingsSet { get; set; }
    }
}
