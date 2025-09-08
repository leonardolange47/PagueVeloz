using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace PagueVeloz.Tests.Integration.Controllers
{
    public class CustomerControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public CustomerControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateCustomer_ShouldReturnOk()
        {
            var response = await _client.PostAsJsonAsync("/api/customer", "João da Silva");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetCustomer_ShouldReturnNotFound_WhenIdIsInvalid()
        {
            var response = await _client.GetAsync($"/api/customer/{Guid.NewGuid()}");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
