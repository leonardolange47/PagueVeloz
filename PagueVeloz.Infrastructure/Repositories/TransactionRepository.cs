using Microsoft.EntityFrameworkCore;
using PagueVeloz.Domain.Entities;
using PagueVeloz.Domain.Repositories;
using PagueVeloz.Infrastructure.Persistence;

namespace PagueVeloz.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly PagueVelozDbContext _context;

        public TransactionRepository(PagueVelozDbContext context)
        {
            _context = context;
        }

        public async Task<Transaction?> GetByReferenceIdAsync(Guid referenceId)
        {
            return await _context.Transactions
                .FirstOrDefaultAsync(t => t.ReferenceId == referenceId);
        }

        public async Task AddAsync(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Transaction>> GetAllByCustomerAsync(Guid customerId)
        {
            var query = from t in _context.Transactions
                        join a in _context.Accounts on t.AccountId equals a.Id
                        join c in _context.Customers on a.CustomerId equals c.Id
                        where c.Id == customerId
                        select t;

            return await query.ToListAsync();
        }

        public async Task AddRangeAsync(IEnumerable<Transaction> transactions)
        {
            await _context.Transactions.AddRangeAsync(transactions);
            await _context.SaveChangesAsync();
        }
    }
}
