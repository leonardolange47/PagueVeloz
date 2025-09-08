using PagueVeloz.Application.Exceptions;
using PagueVeloz.Domain.Entities;
using PagueVeloz.Domain.Enums;
using PagueVeloz.Domain.Repositories;
using PagueVeloz.Domain.Services;

namespace PagueVeloz.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAuditService _auditService;

        public TransactionService(
            IAccountRepository accountRepository,
            ITransactionRepository transactionRepository,
            IAuditService auditService)
        {
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _auditService = auditService;
        }

        public async Task<IEnumerable<Transaction>> GetAllByCustomerAsync(Guid customerId)
        {
            return await _transactionRepository.GetAllByCustomerAsync(customerId);
        }

        public async Task<Transaction> ProcessTransactionAsync(Transaction transaction)
        {
            var existing = await _transactionRepository.GetByReferenceIdAsync(transaction.ReferenceId);
            if (existing != null)
                return existing;

            var account = await _accountRepository.GetByIdAsync(transaction.AccountId);
            if (account == null)
            {
                transaction.MarkFailed("Account not found.");
                await _transactionRepository.AddAsync(transaction);
                return transaction;
            }

            try
            {
                switch (transaction.Type)
                {
                    case TransactionType.Credit:
                        account.Credit(transaction.Amount);
                        break;

                    case TransactionType.Debit:
                        if (!account.Debit(transaction.Amount))
                            throw new BusinessException("Insufficient funds or credit limit exceeded.");
                        break;

                    case TransactionType.Reserve:
                        if (!account.Reserve(transaction.Amount))
                            throw new BusinessException("Insufficient funds for reservation.");
                        break;

                    case TransactionType.Capture:
                        if (!account.Capture(transaction.Amount))
                            throw new BusinessException("Insufficient reserved funds.");
                        break;

                    case TransactionType.Reversal:
                        account.Reverse(transaction.Amount);
                        break;

                    case TransactionType.Transfer:
                        if (transaction.TargetAccountId == null)
                        {
                            throw new BusinessException("Para tranferencia deve ser informada a conta de destino");
                        }
                        var targetAccount = await _accountRepository.GetByIdAsync((Guid)transaction.TargetAccountId);
                        if (account.Balance >= transaction.Amount)
                        {
                            account.Debit(transaction.Amount);
                            targetAccount.Credit(transaction.Amount);
                            await _accountRepository.UpdateAsync(targetAccount);
                        }
                        else
                        {
                            throw new BusinessException("Insufficient funds");
                        }
                        break;
                }

                transaction.MarkSuccess();
                await _transactionRepository.AddAsync(transaction);
                await _accountRepository.UpdateAsync(account);
                await _auditService.LogAsync($"Transaction {transaction.Id} processed successfully.");
                return transaction;
            }
            catch (BusinessException ex)
            {
                transaction.MarkFailed(ex.Message);
                await _transactionRepository.AddAsync(transaction);
                await _auditService.LogAsync($"Transaction {transaction.Id} failed: {ex.Message}");
                return transaction;
            }
        }
    }
}
