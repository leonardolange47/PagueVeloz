using FluentAssertions;
using PagueVeloz.Domain.Entities;
using PagueVeloz.Domain.Enums;

namespace PagueVeloz.Tests.Unit.Domain
{
    public class AccountTests
    {
        [Fact]
        public void Credit_ShouldIncreaseBalance_WhenAccountIsActive()
        {
            var account = new Account(Guid.NewGuid(), 1000);

            account.Credit(500);

            account.Balance.Should().Be(500);
        }

        [Fact]
        public void Credit_ShouldThrow_WhenAccountIsInactive()
        {
            var account = new Account(Guid.NewGuid(), 1000);
            account.Inactivate();

            Action act = () => account.Credit(500);

            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Account is not active.");
        }

        [Fact]
        public void Debit_ShouldDecreaseBalance_WhenFundsAvailable()
        {
            var account = new Account(Guid.NewGuid(), 1000);
            account.Credit(500);

            var result = account.Debit(300);

            result.Should().BeTrue();
            account.Balance.Should().Be(200);
        }

        [Fact]
        public void Debit_ShouldReturnFalse_WhenInsufficientFunds()
        {
            var account = new Account(Guid.NewGuid(), 100);

            var result = account.Debit(500);

            result.Should().BeFalse();
        }

        [Fact]
        public void Reserve_ShouldReserveFunds_WhenEnoughBalance()
        {
            var account = new Account(Guid.NewGuid(), 0);
            account.Credit(1000);

            var result = account.Reserve(500);

            result.Should().BeTrue();
            account.Balance.Should().Be(500);
            account.ReservedBalance.Should().Be(500);
        }

        [Fact]
        public void Reserve_ShouldFail_WhenNotEnoughBalance()
        {
            var account = new Account(Guid.NewGuid(), 0);

            var result = account.Reserve(500);

            result.Should().BeFalse();
        }

        [Fact]
        public void Capture_ShouldReduceReservedBalance_WhenEnoughReserved()
        {
            var account = new Account(Guid.NewGuid(), 0);
            account.Credit(1000);
            account.Reserve(500);

            var result = account.Capture(300);

            result.Should().BeTrue();
            account.ReservedBalance.Should().Be(200);
        }

        [Fact]
        public void Capture_ShouldFail_WhenInsufficientReserved()
        {
            var account = new Account(Guid.NewGuid(), 0);

            var result = account.Capture(100);

            result.Should().BeFalse();
        }

        [Fact]
        public void Reverse_ShouldIncreaseBalance()
        {
            var account = new Account(Guid.NewGuid(), 0);

            account.Reverse(200);

            account.Balance.Should().Be(200);
        }

        [Fact]
        public void Block_ShouldSetStatusToBlocked()
        {
            var account = new Account(Guid.NewGuid(), 0);

            account.Block();

            account.Status.Should().Be(AccountStatus.Blocked);
        }
    }
}
