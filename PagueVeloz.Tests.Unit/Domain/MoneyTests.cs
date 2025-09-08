using FluentAssertions;
using PagueVeloz.Domain.ValueObjects;

namespace PagueVeloz.Tests.Unit.Domain
{
    public class MoneyTests
    {
        [Fact]
        public void Constructor_ShouldAssignValuesCorrectly()
        {
            var money = new Money(1000, "brl");

            money.Amount.Should().Be(1000);
            money.Currency.Should().Be("BRL");
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenCurrencyIsInvalid()
        {
            Action act = () => new Money(1000, "");

            act.Should().Throw<ArgumentException>()
                .WithMessage("Currency required");
        }

        [Fact]
        public void From_ShouldCreateMoneyInstance()
        {
            var money = Money.From(500, "usd");

            money.Amount.Should().Be(500);
            money.Currency.Should().Be("USD");
        }
    }
}
