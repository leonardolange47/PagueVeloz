using Microsoft.AspNetCore.Mvc;
using PagueVeloz.Application.Exceptions;
using PagueVeloz.Domain.Entities;
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

        [HttpPost("batch")]
        public async Task<IActionResult> CreateBatch([FromBody] List<Transaction> transactions)
        {
            if (transactions == null || !transactions.Any())
                return BadRequest("Lista de transações não pode estar vazia.");

            var result = await _transactionService.ProcessTransactionsAsync(transactions);

            if (result == null || !result.Any())
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao processar transações.");

            return Ok(result);
        }
    }
}
