using FluentAssertions;
using PagueVeloz.Domain.Entities;
using PagueVeloz.Infrastructure.Repositories;

namespace PagueVeloz.Tests.Unit.Infrastructure
{
    public class AccountRepositoryTests
    {
        [Fact]
        public async Task AddAsync_Should_Add_Account_To_Database()
        {
            var context = DbContextHelper.CreateInMemoryDbContext();
            var repo = new AccountRepository(context);
            var account = new Account(Guid.NewGuid(), 1000);

            await repo.AddAsync(account);

            var dbAccount = await context.Accounts.FindAsync(account.Id);
            dbAccount.Should().NotBeNull();
            dbAccount!.CreditLimit.Should().Be(1000);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Account_When_Exists()
        {
            var context = DbContextHelper.CreateInMemoryDbContext();
            var repo = new AccountRepository(context);
            var account = new Account(Guid.NewGuid(), 500);
            await context.Accounts.AddAsync(account);
            await context.SaveChangesAsync();

            var dbAccount = await repo.GetByIdAsync(account.Id);

            dbAccount.Should().NotBeNull();
            dbAccount!.Id.Should().Be(account.Id);
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_Account_Data()
        {
            var context = DbContextHelper.CreateInMemoryDbContext();
            var repo = new AccountRepository(context);
            var account = new Account(Guid.NewGuid(), 200);
            await context.Accounts.AddAsync(account);
            await context.SaveChangesAsync();

            account.Credit(500);
            await repo.UpdateAsync(account);

            var updatedAccount = await context.Accounts.FindAsync(account.Id);
            updatedAccount!.Balance.Should().Be(500);
        }
    }
}
