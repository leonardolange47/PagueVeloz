using FluentAssertions;
using PagueVeloz.Domain.Entities;
using PagueVeloz.Domain.Enums;
using PagueVeloz.Infrastructure.Repositories;

namespace PagueVeloz.Tests.Unit.Infrastructure
{
    public class TransactionRepositoryTests
    {
        [Fact]
        public async Task AddAsync_Should_Add_Transaction_To_Database()
        {
            var context = DbContextHelper.CreateInMemoryDbContext();
            var repo = new TransactionRepository(context);
            var transaction = new Transaction(Guid.NewGuid(), TransactionType.Credit, 100, "BRL", "test", null);

            await repo.AddAsync(transaction);

            var dbTransaction = await context.Transactions.FindAsync(transaction.Id);
            dbTransaction.Should().NotBeNull();
            dbTransaction!.Amount.Should().Be(100);
        }

        [Fact]
        public async Task GetByReferenceIdAsync_Should_Return_Transaction_When_Exists()
        {
            var context = DbContextHelper.CreateInMemoryDbContext();
            var repo = new TransactionRepository(context);
            var transaction = new Transaction(Guid.NewGuid(), TransactionType.Credit, 200, "USD", "metadata", null);
            await context.Transactions.AddAsync(transaction);
            await context.SaveChangesAsync();

            var dbTransaction = await repo.GetByReferenceIdAsync(transaction.ReferenceId);

            dbTransaction.Should().NotBeNull();
            dbTransaction!.ReferenceId.Should().Be(transaction.ReferenceId);
        }

        [Fact]
        public async Task GetAllByCustomerAsync_Should_Return_Transactions_For_Customer()
        {
            var context = DbContextHelper.CreateInMemoryDbContext();
            var repo = new TransactionRepository(context);

            var customer = new Customer("John Doe");
            var account = new Account(customer.Id, 1000);
            var transaction = new Transaction(account.Id, TransactionType.Debit, 50, "BRL", "test", null);

            await context.Customers.AddAsync(customer);
            await context.Accounts.AddAsync(account);
            await context.Transactions.AddAsync(transaction);
            await context.SaveChangesAsync();

            var transactions = await repo.GetAllByCustomerAsync(customer.Id);

            transactions.Should().HaveCount(1);
            transactions.First().Amount.Should().Be(50);
        }
    }
}
