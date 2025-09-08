using FluentAssertions;
using PagueVeloz.Domain.Entities;

namespace PagueVeloz.Tests.Unit.Domain
{
    public class CustomerTests
    {
        [Fact]
        public void Constructor_ShouldAssignNameAndId()
        {
            var customer = new Customer("John Doe");

            customer.Name.Should().Be("John Doe");
            customer.Id.Should().NotBeEmpty();
        }
    }
}
