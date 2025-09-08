using FluentAssertions;
using Moq;
using PagueVeloz.Application.Services;
using PagueVeloz.Domain.Entities;
using PagueVeloz.Domain.Repositories;

namespace PagueVeloz.Tests.Unit.Application
{
    public class AccountServiceTests
    {
        private readonly Mock<IAccountRepository> _accountRepoMock;
        private readonly AccountService _service;

        public AccountServiceTests()
        {
            _accountRepoMock = new Mock<IAccountRepository>();
            _service = new AccountService(_accountRepoMock.Object);
        }

        [Fact]
        public async Task CreateAccountAsync_Should_Create_And_Save_Account()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            decimal creditLimit = 1000;

            // Act
            var result = await _service.CreateAccountAsync(customerId, creditLimit);

            // Assert
            result.CustomerId.Should().Be(customerId);
            result.CreditLimit.Should().Be(creditLimit);
            _accountRepoMock.Verify(r => r.AddAsync(It.IsAny<Account>()), Times.Once);
        }

        [Fact]
        public async Task GetAccountAsync_Should_Return_Account_When_Exists()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var account = new Account(Guid.NewGuid(), 500);
            _accountRepoMock.Setup(r => r.GetByIdAsync(accountId))
                            .ReturnsAsync(account);

            // Act
            var result = await _service.GetAccountAsync(accountId);

            // Assert
            result.Should().Be(account);
        }
    }
}
