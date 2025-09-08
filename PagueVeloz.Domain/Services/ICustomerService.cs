using PagueVeloz.Domain.Entities;

namespace PagueVeloz.Domain.Services
{
    public interface ICustomerService
    {
        Task<Customer?> GetByIdAsync(Guid id);
        Task AddAsync(Customer customer);
    }
}
