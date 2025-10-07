using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using UAE_Pass_Poc.DBContext;

namespace UAE_Pass_Poc.DBContext
{
    public class UaePassDbContextFactory : IDesignTimeDbContextFactory<UaePassDbContext>
    {
        public UaePassDbContext CreateDbContext(string[] args)
        {
            // Build configuration
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // Get connection string
            var connectionString = configuration.GetConnectionString("DbConnection");

            var optionsBuilder = new DbContextOptionsBuilder<UaePassDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new UaePassDbContext(optionsBuilder.Options);
        }
    }
}