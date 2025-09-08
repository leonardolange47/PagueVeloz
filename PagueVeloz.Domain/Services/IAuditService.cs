namespace PagueVeloz.Domain.Services
{
    public interface IAuditService
    {
        Task LogAsync(string message);
    }
}
