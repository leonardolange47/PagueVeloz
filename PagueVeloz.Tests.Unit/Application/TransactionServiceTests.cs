using FluentAssertions;
using Moq;
using PagueVeloz.Application.Exceptions;
using PagueVeloz.Application.Services;
using PagueVeloz.Domain.Entities;
using PagueVeloz.Domain.Enums;
using PagueVeloz.Domain.Repositories;
using PagueVeloz.Domain.Services;

namespace PagueVeloz.Tests.Unit.Application
{
    public class TransactionServiceFullTests
    {
        private readonly Mock<IAccountRepository> _accountRepoMock;
        private readonly Mock<ITransactionRepository> _transactionRepoMock;
        private readonly Mock<IAuditService> _auditServiceMock;
        private readonly TransactionService _service;

        public TransactionServiceFullTests()
        {
            _accountRepoMock = new Mock<IAccountRepository>();
            _transactionRepoMock = new Mock<ITransactionRepository>();
            _auditServiceMock = new Mock<IAuditService>();

            _service = new TransactionService(
                _accountRepoMock.Object,
                _transactionRepoMock.Object,
                _auditServiceMock.Object
            );
        }

        private static Transaction CreateTransaction(Guid accountId, TransactionType type, decimal amount, Guid? targetAccountId = null)
        {
            return new Transaction(
                accountId,
                type,
                amount,
                "BRL",
                "Test",
                targetAccountId
            );
        }

        // ---------------- CREDIT ----------------

        [Fact]
        public async Task Credit_Should_Increase_Balance()
        {
            var account = new Account(Guid.NewGuid(), 0);
            var transaction = CreateTransaction(account.Id, TransactionType.Credit, 1000);

            _accountRepoMock.Setup(r => r.GetByIdAsync(account.Id)).ReturnsAsync(account);

            var result = await _service.ProcessTransactionAsync(transaction);

            result.Status.Should().Be(TransactionStatus.Success);
            account.Balance.Should().Be(1000);
        }

        [Fact]
        public async Task Credit_Should_Fail_When_Account_Not_Found()
        {
            var transaction = CreateTransaction(Guid.NewGuid(), TransactionType.Credit, 1000);

            _accountRepoMock.Setup(r => r.GetByIdAsync(transaction.AccountId)).ReturnsAsync((Account?)null);

            var result = await _service.ProcessTransactionAsync(transaction);

            result.Status.Should().Be(TransactionStatus.Failed);
            result.ErrorMessage.Should().Contain("Account not found");
        }

        // ---------------- DEBIT ----------------

        [Fact]
        public async Task Debit_Should_Decrease_Balance()
        {
            var account = new Account(Guid.NewGuid(), 0);
            account.Credit(1000);
            var transaction = CreateTransaction(account.Id, TransactionType.Debit, 500);

            _accountRepoMock.Setup(r => r.GetByIdAsync(account.Id)).ReturnsAsync(account);

            var result = await _service.ProcessTransactionAsync(transaction);

            result.Status.Should().Be(TransactionStatus.Success);
            account.Balance.Should().Be(500);
        }

        [Fact]
        public async Task Debit_Should_Fail_When_Insufficient_Funds()
        {
            var account = new Account(Guid.NewGuid(), 0);
            var transaction = CreateTransaction(account.Id, TransactionType.Debit, 500);

            _accountRepoMock.Setup(r => r.GetByIdAsync(account.Id)).ReturnsAsync(account);

            var result = await _service.ProcessTransactionAsync(transaction);

            result.Status.Should().Be(TransactionStatus.Failed);
            result.ErrorMessage.Should().Contain("Insufficient");
        }

        // ---------------- RESERVE ----------------

        [Fact]
        public async Task Reserve_Should_Reserve_Amount()
        {
            var account = new Account(Guid.NewGuid(), 0);
            account.Credit(1000);
            var transaction = CreateTransaction(account.Id, TransactionType.Reserve, 500);

            _accountRepoMock.Setup(r => r.GetByIdAsync(account.Id)).ReturnsAsync(account);

            var result = await _service.ProcessTransactionAsync(transaction);

            result.Status.Should().Be(TransactionStatus.Success);
            account.ReservedBalance.Should().Be(500);
            account.Balance.Should().Be(500);
        }

        [Fact]
        public async Task Reserve_Should_Fail_When_Insufficient_Balance()
        {
            var account = new Account(Guid.NewGuid(), 0);
            var transaction = CreateTransaction(account.Id, TransactionType.Reserve, 500);

            _accountRepoMock.Setup(r => r.GetByIdAsync(account.Id)).ReturnsAsync(account);

            var result = await _service.ProcessTransactionAsync(transaction);

            result.Status.Should().Be(TransactionStatus.Failed);
            result.ErrorMessage.Should().Contain("Insufficient");
        }

        // ---------------- CAPTURE ----------------

        [Fact]
        public async Task Capture_Should_Decrease_Reserved()
        {
            var account = new Account(Guid.NewGuid(), 0);
            account.Credit(1000);
            account.Reserve(500);
            var transaction = CreateTransaction(account.Id, TransactionType.Capture, 500);

            _accountRepoMock.Setup(r => r.GetByIdAsync(account.Id)).ReturnsAsync(account);

            var result = await _service.ProcessTransactionAsync(transaction);

            result.Status.Should().Be(TransactionStatus.Success);
            account.ReservedBalance.Should().Be(0);
            account.Balance.Should().Be(500);
        }

        [Fact]
        public async Task Capture_Should_Fail_When_Insufficient_Reserved()
        {
            var account = new Account(Guid.NewGuid(), 0);
            var transaction = CreateTransaction(account.Id, TransactionType.Capture, 500);

            _accountRepoMock.Setup(r => r.GetByIdAsync(account.Id)).ReturnsAsync(account);

            var result = await _service.ProcessTransactionAsync(transaction);

            result.Status.Should().Be(TransactionStatus.Failed);
            result.ErrorMessage.Should().Contain("Insufficient");
        }

        // ---------------- REVERSAL ----------------

        [Fact]
        public async Task Reversal_Should_Increase_Balance()
        {
            var account = new Account(Guid.NewGuid(), 0);
            var transaction = CreateTransaction(account.Id, TransactionType.Reversal, 500);

            _accountRepoMock.Setup(r => r.GetByIdAsync(account.Id)).ReturnsAsync(account);

            var result = await _service.ProcessTransactionAsync(transaction);

            result.Status.Should().Be(TransactionStatus.Success);
            account.Balance.Should().Be(500);
        }

        [Fact]
        public async Task Reversal_Should_Fail_When_Account_Not_Found()
        {
            var transaction = CreateTransaction(Guid.NewGuid(), TransactionType.Reversal, 500);

            _accountRepoMock.Setup(r => r.GetByIdAsync(transaction.AccountId)).ReturnsAsync((Account?)null);

            var result = await _service.ProcessTransactionAsync(transaction);

            result.Status.Should().Be(TransactionStatus.Failed);
        }

        // ---------------- TRANSFER ----------------

        [Fact]
        public async Task Transfer_Should_Move_Funds()
        {
            var source = new Account(Guid.NewGuid(), 0);
            source.Credit(1000);
            var target = new Account(Guid.NewGuid(), 0);

            var transaction = CreateTransaction(source.Id, TransactionType.Transfer, 500, target.Id);

            _accountRepoMock.Setup(r => r.GetByIdAsync(source.Id)).ReturnsAsync(source);
            _accountRepoMock.Setup(r => r.GetByIdAsync(target.Id)).ReturnsAsync(target);

            var result = await _service.ProcessTransactionAsync(transaction);

            result.Status.Should().Be(TransactionStatus.Success);
            source.Balance.Should().Be(500);
            target.Balance.Should().Be(500);
        }

        [Fact]
        public async Task Transfer_Should_Fail_When_No_TargetAccount()
        {
            var source = new Account(Guid.NewGuid(), 0);
            source.Credit(1000);

            var transaction = CreateTransaction(source.Id, TransactionType.Transfer, 500);

            _accountRepoMock.Setup(r => r.GetByIdAsync(source.Id)).ReturnsAsync(source);

            try
            {
                var result = await _service.ProcessTransactionAsync(transaction);
            }
            catch (BusinessException ex)
            {
                ex.Message.Should().Contain("Para tranferencia deve ser informada a conta de destino");

            }
        }
    }
}
