using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PagueVeloz.Domain.Entities;

namespace PagueVeloz.Infrastructure.Persistence.Configurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Balance)
                .HasColumnType("decimal(18,2)");

            builder.Property(a => a.ReservedBalance)
                .HasColumnType("decimal(18,2)");

            builder.Property(a => a.CreditLimit)
                .HasColumnType("decimal(18,2)");

            builder.HasMany(a => a.Transactions)
                .WithOne()
                .HasForeignKey(t => t.AccountId);
        }
    }
}
