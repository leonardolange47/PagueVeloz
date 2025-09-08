using PagueVeloz.Domain.Entities;
using PagueVeloz.Domain.Repositories;
using PagueVeloz.Domain.Services;

namespace PagueVeloz.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<Account> CreateAccountAsync(Guid customerId, decimal creditLimit)
        {
            var account = new Account(customerId, creditLimit);
            await _accountRepository.AddAsync(account);
            return account;
        }

        public async Task<Account?> GetAccountAsync(Guid accountId)
        {
            return await _accountRepository.GetByIdAsync(accountId);
        }
    }
}
