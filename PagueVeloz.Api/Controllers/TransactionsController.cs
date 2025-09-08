using Microsoft.AspNetCore.Mvc;
using PagueVeloz.Application.Exceptions;
using PagueVeloz.Domain.Services;

namespace PagueVeloz.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Domain.Entities.Transaction request)
        {
            try
            {
                var result = await _transactionService.ProcessTransactionAsync(request);
                return Ok(result);
            }
            catch (BusinessException ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var account = await _transactionService.GetAllByCustomerAsync(id);
            if (account == null)
                return NotFound();

            return Ok(account);
        }
    }
}
