using FluentAssertions;
using Moq;
using PagueVeloz.Application.Services;
using PagueVeloz.Domain.Entities;
using PagueVeloz.Domain.Repositories;

namespace PagueVeloz.Tests.Unit.Application
{
    public class CustomerServiceTests
    {
        private readonly Mock<ICustomerRepository> _customerRepoMock;
        private readonly CustomerService _service;

        public CustomerServiceTests()
        {
            _customerRepoMock = new Mock<ICustomerRepository>();
            _service = new CustomerService(_customerRepoMock.Object);
        }

        [Fact]
        public async Task AddAsync_Should_Call_Repository()
        {
            // Arrange
            var customer = new Customer("Test");

            // Act
            await _service.AddAsync(customer);

            // Assert
            _customerRepoMock.Verify(r => r.AddAsync(customer), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Customer()
        {
            // Arrange
            var id = Guid.NewGuid();
            var customer = new Customer("Test");
            _customerRepoMock.Setup(r => r.GetByIdAsync(id))
                             .ReturnsAsync(customer);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            result.Should().Be(customer);
        }
    }
}
