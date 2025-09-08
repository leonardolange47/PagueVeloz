using FluentAssertions;
using PagueVeloz.Domain.Entities;
using System.Net;
using System.Net.Http.Json;

namespace PagueVeloz.Tests.Integration.Controllers
{
    public class AccountsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public AccountsControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateAccount_ShouldReturnOk()
        {
            var customerId = Guid.NewGuid();
            var account = new Account(customerId, 1000m);

            var response = await _client.PostAsJsonAsync("/api/accounts", account);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var created = await response.Content.ReadFromJsonAsync<Account>();
            created.Should().NotBeNull();
            created!.CustomerId.Should().Be(customerId);
        }

        [Fact]
        public async Task GetAccount_ShouldReturnNotFound_WhenIdIsInvalid()
        {
            var response = await _client.GetAsync($"/api/accounts/{Guid.NewGuid()}");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
