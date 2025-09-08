using PagueVeloz.Domain.Entities;

namespace PagueVeloz.Domain.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByIdAsync(Guid id);
        Task AddAsync(Customer customer);
    }
}
