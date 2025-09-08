using FluentAssertions;
using PagueVeloz.Application.Services;

namespace PagueVeloz.Tests.Unit.Application
{
    public class AuditServiceTests
    {
        [Fact]
        public async Task LogAsync_Should_Complete_Without_Exception()
        {
            // Arrange
            var auditService = new AuditService();

            // Act
            var exception = await Record.ExceptionAsync(() => auditService.LogAsync("Test message"));

            // Assert
            exception.Should().BeNull();
        }
    }
}
