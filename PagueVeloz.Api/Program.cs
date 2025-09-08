
using Microsoft.EntityFrameworkCore;
using PagueVeloz.Application.Services;
using PagueVeloz.Domain.Repositories;
using PagueVeloz.Domain.Services;
using PagueVeloz.Infrastructure.Persistence;
using PagueVeloz.Infrastructure.Repositories;

namespace PagueVeloz.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // -----------------------
            // Configuração do DbContext
            // -----------------------
            if (builder.Environment.EnvironmentName == "Testing")
            {
                // Para testes de integração com InMemory
                builder.Services.AddDbContext<PagueVelozDbContext>(options =>
                    options.UseInMemoryDatabase("IntegrationTestsDb"));
            }
            else
            {
                // Para execução normal com SQL Server
                builder.Services.AddDbContext<PagueVelozDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            }

            // -----------------------
            // Dependency Injection
            // -----------------------

            // Repositories
            builder.Services.AddScoped<IAccountRepository, AccountRepository>();
            builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

            // Services
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<ITransactionService, TransactionService>();
            builder.Services.AddScoped<IAuditService, AuditService>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();

            // -----------------------
            // Controllers & Swagger
            // -----------------------
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "PagueVeloz Transaction API",
                    Version = "v1",
                    Description = "API REST para processamento de transações financeiras"
                });
            });

            var app = builder.Build();

            // -----------------------
            // Middleware
            // -----------------------
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "PagueVeloz Transaction API v1");
                c.RoutePrefix = string.Empty; // Swagger na raiz: http://localhost:5000/
            });

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
