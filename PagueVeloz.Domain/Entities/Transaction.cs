using PagueVeloz.Domain.Enums;

namespace PagueVeloz.Domain.Entities
{
    public class Transaction
    {
        public Guid Id { get; private set; }
        public Guid AccountId { get; private set; }
        public TransactionType Type { get; private set; }
        public decimal Amount { get; private set; }
        public string Currency { get; private set; }
        public Guid ReferenceId { get; private set; }
        public Enums.TransactionStatus Status { get; private set; }
        public DateTime Timestamp { get; private set; }
        public string? ErrorMessage { get; private set; }
        public string Metadata { get; private set; }
        public Guid? TargetAccountId { get; set; }

        public Transaction(Guid accountId, TransactionType type, decimal amount, string currency, string metadata, Guid? targetAccountId)
        {
            Id = Guid.NewGuid();
            AccountId = accountId;
            Type = type;
            Amount = amount;
            Currency = currency;
            ReferenceId = Guid.NewGuid();
            Status = Enums.TransactionStatus.Pending;
            Timestamp = DateTime.UtcNow;
            Metadata = metadata;
            TargetAccountId = targetAccountId;
        }

        public void MarkSuccess()
        {
            Status = Enums.TransactionStatus.Success;
            ErrorMessage = null;
        }

        public void MarkFailed(string error)
        {
            Status = Enums.TransactionStatus.Failed;
            ErrorMessage = error;
        }
    }
}
