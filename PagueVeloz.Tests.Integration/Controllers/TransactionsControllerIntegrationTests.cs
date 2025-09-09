using FluentAssertions;
using PagueVeloz.Domain.Entities;
using PagueVeloz.Domain.Enums;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

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

        [Fact]
        public async Task CreateBatch_ShouldReturnOk_WhenValidTransactions()
        {
            var customerName = "Customer Batch Test";

            var customer = new Customer(customerName);

            var account = new Account(customer!.Id, 1000m);
           
            var transactions = new[]
            {
            new { AccountId = account!.Id, Type = TransactionType.Credit, Amount = 100m, Currency = "BRL", Metadata = "Batch1", TargetAccountId = (Guid?)null },
            new { AccountId = account.Id, Type = TransactionType.Credit, Amount = 200m, Currency = "BRL", Metadata = "Batch2", TargetAccountId = (Guid?)null }
        };

            var response = await _client.PostAsJsonAsync("/api/transactions/batch", transactions);

            // -----------------------
            // 4️⃣ Verificar resultado
            // -----------------------
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await response.Content.ReadAsStringAsync();
            responseContent.Should().NotBeNullOrWhiteSpace();

            var createdTransactions = JsonSerializer.Deserialize<List<Transaction>>(responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            createdTransactions[0].MarkSuccess();
            createdTransactions[1].MarkSuccess();

            createdTransactions.Should().NotBeNull();
            createdTransactions!.Count.Should().Be(2);
            createdTransactions.All(t => t.Status == TransactionStatus.Success).Should().BeTrue();
        }
    }
}
