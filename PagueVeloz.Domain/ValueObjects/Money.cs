namespace PagueVeloz.Domain.ValueObjects
{
    public sealed class Money
    {
        public long Amount { get; }    // em centavos
        public string Currency { get; }

        public Money(long amount, string currency)
        {
            if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("Currency required");
            Amount = amount;
            Currency = currency.ToUpperInvariant();
        }

        public static Money From(long amount, string currency) => new Money(amount, currency);
    }
}
