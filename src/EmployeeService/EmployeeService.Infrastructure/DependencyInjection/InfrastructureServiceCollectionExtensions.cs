using EmployeeService.Application.Interfaces.Messaging;
using EmployeeService.Application.Interfaces.Repositories;
using EmployeeService.Infrastructure.Messaging;
using EmployeeService.Infrastructure.Persistence.DbContexts;
using EmployeeService.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeService.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<EmployeeDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("HrDatabase")));

        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IOnboardingRepository, OnboardingRepository>();
        services.AddSingleton<IIntegrationEventPublisher, RabbitMqEventPublisher>();
        
        return services;
    }
}
