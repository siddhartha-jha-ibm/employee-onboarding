using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PayrollService.Application.Interfaces.Messaging;
using PayrollService.Application.Interfaces.Repositories;
using PayrollService.Infrastructure.Messaging;
using PayrollService.Infrastructure.Persistence.DbContexts;
using PayrollService.Infrastructure.Persistence.Repositories;

namespace PayrollService.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<PayrollDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("HrDatabase")));

        services.AddScoped<IPayrollRepository, PayrollRepository>();
        services.AddSingleton<IIntegrationEventPublisher, RabbitMqEventPublisher>();

        return services;
    }
}