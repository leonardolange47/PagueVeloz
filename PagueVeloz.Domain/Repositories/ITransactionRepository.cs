using PagueVeloz.Domain.Entities;

namespace PagueVeloz.Domain.Repositories
{
    public interface ITransactionRepository
    {
        Task<Transaction?> GetByReferenceIdAsync(Guid referenceId);
        Task AddAsync(Transaction transaction);
        Task<IEnumerable<Transaction>> GetAllByCustomerAsync(Guid customerId);
    }
}
