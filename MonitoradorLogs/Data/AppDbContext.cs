using Microsoft.EntityFrameworkCore;
using MonitoradorLogs.Models;

namespace MonitoradorLogs.Data
{
    public class AppDbContext :DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Log> Log { get; set; }        

    }
}
