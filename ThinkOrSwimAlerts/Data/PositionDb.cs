using Microsoft.EntityFrameworkCore;
using ThinkOrSwimAlerts.Data.Models;

namespace ThinkOrSwimAlerts.Data
{
    public class PositionDb : DbContext
    {
        public PositionDb(DbContextOptions<PositionDb> options) : base( options )
        {
        }

        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<PositionUpdate> PositionUpdates { get; set; }
    }
}
