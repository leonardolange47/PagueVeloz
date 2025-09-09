using PagueVeloz.Domain.Entities;

namespace PagueVeloz.Domain.Services
{
    public interface ITransactionService
    {
        Task<Transaction> ProcessTransactionAsync(Transaction transaction);
        Task<IEnumerable<Transaction>> GetAllByCustomerAsync(Guid customerId);
        Task<List<Transaction>> ProcessTransactionsAsync(List<Transaction> transactions);
    }
}
