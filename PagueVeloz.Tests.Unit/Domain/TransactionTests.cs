using FluentAssertions;
using PagueVeloz.Domain.Entities;
using PagueVeloz.Domain.Enums;

namespace PagueVeloz.Tests.Unit.Domain
{
    public class TransactionTests
    {
        [Fact]
        public void Constructor_ShouldAssignValuesCorrectly()
        {
            var accountId = Guid.NewGuid();
            var transaction = new Transaction(accountId, TransactionType.Credit, 100, "BRL", "metadata", null);

            transaction.AccountId.Should().Be(accountId);
            transaction.Type.Should().Be(TransactionType.Credit);
            transaction.Amount.Should().Be(100);
            transaction.Currency.Should().Be("BRL");
            transaction.Status.Should().Be(TransactionStatus.Pending);
            transaction.Metadata.Should().Be("metadata");
        }

        [Fact]
        public void MarkSuccess_ShouldSetStatusToSuccess()
        {
            var transaction = new Transaction(Guid.NewGuid(), TransactionType.Credit, 100, "BRL", "test", null);

            transaction.MarkSuccess();

            transaction.Status.Should().Be(TransactionStatus.Success);
            transaction.ErrorMessage.Should().BeNull();
        }

        [Fact]
        public void MarkFailed_ShouldSetStatusToFailed()
        {
            var transaction = new Transaction(Guid.NewGuid(), TransactionType.Credit, 100, "BRL", "test", null);

            transaction.MarkFailed("error");

            transaction.Status.Should().Be(TransactionStatus.Failed);
            transaction.ErrorMessage.Should().Be("error");
        }
    }
}
