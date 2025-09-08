using Microsoft.AspNetCore.Mvc;
using PagueVeloz.Domain.Entities;
using PagueVeloz.Domain.Services;

namespace PagueVeloz.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        public CustomerController(ICustomerService costumerService)
        {
            _customerService = costumerService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] string name)
        {
            var customer = new Customer(name);
            await _customerService.AddAsync(customer);
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null)
                return NotFound();

            return Ok(customer);
        }
    }
}
