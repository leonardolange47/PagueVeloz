using Microsoft.EntityFrameworkCore;
using PagueVeloz.Domain.Entities;

namespace PagueVeloz.Infrastructure.Persistence
{
    public class PagueVelozDbContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Customer> Customers { get; set; }

        public PagueVelozDbContext(DbContextOptions<PagueVelozDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PagueVelozDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
