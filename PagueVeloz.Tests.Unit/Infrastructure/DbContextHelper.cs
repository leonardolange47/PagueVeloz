using Microsoft.EntityFrameworkCore;
using PagueVeloz.Infrastructure.Persistence;

namespace PagueVeloz.Tests.Unit.Infrastructure
{
    public static class DbContextHelper
    {
        public static PagueVelozDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<PagueVelozDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // DB único para cada teste
                .Options;

            return new PagueVelozDbContext(options);
        }
    }
}
