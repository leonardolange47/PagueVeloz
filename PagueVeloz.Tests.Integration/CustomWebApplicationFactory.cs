using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PagueVeloz.Api;
using PagueVeloz.Application.Services;
using PagueVeloz.Domain.Repositories;
using PagueVeloz.Domain.Services;
using PagueVeloz.Infrastructure.Persistence;
using PagueVeloz.Infrastructure.Repositories;

namespace PagueVeloz.Tests.Integration
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing"); // Define ambiente de teste

            builder.ConfigureServices(services =>
            {
                // Remove DbContext real (SQL Server)
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<PagueVelozDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // Adiciona DbContext em memória para testes
                services.AddDbContext<PagueVelozDbContext>(options =>
                {
                    options.UseInMemoryDatabase("IntegrationTestsDb");
                });

                // Registra repositórios e serviços
                services.AddScoped<IAccountRepository, AccountRepository>();
                services.AddScoped<IAccountService, AccountService>();
                services.AddScoped<ICustomerRepository, CustomerRepository>();
                services.AddScoped<ICustomerService, CustomerService>();
                services.AddScoped<ITransactionRepository, TransactionRepository>();
                services.AddScoped<ITransactionService, TransactionService>();
                services.AddScoped<IAuditService, AuditService>();

                // Build ServiceProvider para inicializar banco InMemory
                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<PagueVelozDbContext>();
                    var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory>>();

                    // Garante que o banco de teste esteja limpo
                    db.Database.EnsureDeleted();
                    db.Database.EnsureCreated();

                    try
                    {
                        // Seed de dados iniciais, se necessário
                        // db.Customers.Add(new Customer("Test Customer"));
                        // db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Erro ao inicializar banco de teste.");
                    }
                }
            });
        }
    }
}
