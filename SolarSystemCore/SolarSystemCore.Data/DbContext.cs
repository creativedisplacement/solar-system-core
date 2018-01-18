using Microsoft.EntityFrameworkCore;
using SolarSystemCore.Models;

namespace SolarSystemCore.Data
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
