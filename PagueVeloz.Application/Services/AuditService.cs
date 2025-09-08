using PagueVeloz.Domain.Services;

namespace PagueVeloz.Application.Services
{
    public class AuditService : IAuditService
    {
        public Task LogAsync(string message)
        {
            Console.WriteLine($"[AUDIT] {DateTime.UtcNow}: {message}");
            return Task.CompletedTask;
        }
    }
}
