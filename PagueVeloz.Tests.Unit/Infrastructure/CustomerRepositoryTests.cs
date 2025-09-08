using FluentAssertions;
using PagueVeloz.Domain.Entities;
using PagueVeloz.Infrastructure.Repositories;

namespace PagueVeloz.Tests.Unit.Infrastructure
{
    public class CustomerRepositoryTests
    {
        [Fact]
        public async Task AddAsync_Should_Add_Customer_To_Database()
        {
            var context = DbContextHelper.CreateInMemoryDbContext();
            var repo = new CustomerRepository(context);
            var customer = new Customer("John Doe");

            await repo.AddAsync(customer);

            var dbCustomer = await context.Customers.FindAsync(customer.Id);
            dbCustomer.Should().NotBeNull();
            dbCustomer!.Name.Should().Be("John Doe");
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Customer_When_Exists()
        {
            var context = DbContextHelper.CreateInMemoryDbContext();
            var repo = new CustomerRepository(context);
            var customer = new Customer("Jane Doe");
            await context.Customers.AddAsync(customer);
            await context.SaveChangesAsync();

            var dbCustomer = await repo.GetByIdAsync(customer.Id);

            dbCustomer.Should().NotBeNull();
            dbCustomer!.Id.Should().Be(customer.Id);
        }
    }
}
