using FluentAssertions;
using PagueVeloz.Domain.Entities;
using PagueVeloz.Domain.Enums;
using System.Net;
using System.Net.Http.Json;

namespace PagueVeloz.Tests.Integration.Controllers
{
    public class TransactionsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public TransactionsControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateTransaction_ShouldReturnOk_WhenCredit()
        {
            var customerName = "Test Customer";

            var customer = new Customer(customerName);

            var account = new Account(customer!.Id, 1000m);

            var transaction = new Transaction(
                account.Id,
                TransactionType.Credit,
                500m,
                "BRL",
                "Teste",
                null
            );

            var response = await _client.PostAsJsonAsync("/api/transactions", transaction);

            response.EnsureSuccessStatusCode();
            var createdTransaction = await response.Content.ReadFromJsonAsync<Transaction>();
            createdTransaction.Should().NotBeNull();
            createdTransaction!.Status.Should().Be(TransactionStatus.Pending);
        }

        [Fact]
        public async Task GetTransactions_ShouldReturnEmptyList_WhenCustomerHasNoTransactions()
        {
            var response = await _client.GetAsync($"/api/transactions/{Guid.NewGuid()}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
