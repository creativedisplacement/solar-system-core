using Microsoft.EntityFrameworkCore;
using SolarSystemCore.Core.Entities;

namespace SolarSystemCore.Infrastructure.Data
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {
            
        }
        public virtual DbSet<Star> Stars { get; set; }
        public virtual DbSet<Planet> Planets { get; set; }
        public virtual DbSet<Moon> Moons { get; set; }
    }
}
