using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace PagueVeloz.Infrastructure.Persistence
{
    public class PagueVelozDbContextFactory : IDesignTimeDbContextFactory<PagueVelozDbContext>
    {
        public PagueVelozDbContext CreateDbContext(string[] args)
        {
            // Localiza o appsettings.json do projeto API
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../PagueVeloz.API");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<PagueVelozDbContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

            return new PagueVelozDbContext(optionsBuilder.Options);
        }
    }
}
