using PagueVeloz.Domain.Enums;

namespace PagueVeloz.Domain.Entities
{
    public class Account
    {
        public Guid Id { get; private set; }
        public Guid CustomerId { get; private set; }
        public decimal Balance { get; private set; }
        public decimal ReservedBalance { get; private set; }
        public decimal CreditLimit { get; private set; }
        public AccountStatus Status { get; private set; }

        private readonly List<Transaction> _transactions = new();
        public IReadOnlyCollection<Transaction> Transactions => _transactions.AsReadOnly();

        public Account(Guid customerId, decimal creditLimit)
        {
            Id = Guid.NewGuid();
            CustomerId = customerId;
            CreditLimit = creditLimit;
            Balance = 0;
            ReservedBalance = 0;
            Status = AccountStatus.Active;
        }

        public void Credit(decimal amount)
        {
            if (Status != AccountStatus.Active)
                throw new InvalidOperationException("Account is not active.");
            Balance += amount;
        }

        public bool Debit(decimal amount)
        {
            if (Status != AccountStatus.Active)
                throw new InvalidOperationException("Account is not active.");

            if (Balance + CreditLimit < amount)
                return false;

            Balance -= amount;
            return true;
        }

        public bool Reserve(decimal amount)
        {
            if (amount > Balance)
                return false;

            Balance -= amount;
            ReservedBalance += amount;
            return true;
        }

        public bool Capture(decimal amount)
        {
            if (amount > ReservedBalance)
                return false;

            ReservedBalance -= amount;
            return true;
        }

        public void Reverse(decimal amount)
        {
            Balance += amount;
        }

        public void Block() => Status = AccountStatus.Blocked;
        public void Inactivate() => Status = AccountStatus.Inactive;
    }
}
