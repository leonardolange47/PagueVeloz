using PagueVeloz.Domain.Entities;

namespace PagueVeloz.Domain.Services
{
    public interface IAccountService
    {
        Task<Account> CreateAccountAsync(Guid customerId, decimal creditLimit);
        Task<Account?> GetAccountAsync(Guid accountId);
    }
}
