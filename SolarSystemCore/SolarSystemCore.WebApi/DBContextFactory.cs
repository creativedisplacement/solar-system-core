using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SolarSystemCore.Data;
using System.IO;

namespace SolarSystemCore.WebApi
{
    public class DBContextFactory : IDesignTimeDbContextFactory<DBContext>
    {
        public DBContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var builder = new DbContextOptionsBuilder<DBContext>();
            var connectionString = configuration.GetValue<string>("Data:ConnectionString");
            builder.UseSqlServer(connectionString);
            return new DBContext(builder.Options);
        }
    }
}
