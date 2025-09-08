using Microsoft.AspNetCore.Mvc;
using PagueVeloz.Domain.Services;

namespace PagueVeloz.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Domain.Entities.Account dto)
        {
            var account = await _accountService.CreateAccountAsync(dto.CustomerId, dto.CreditLimit);
            return Ok(account);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var account = await _accountService.GetAccountAsync(id);
            if (account == null)
                return NotFound();

            return Ok(account);
        }
    }
}
