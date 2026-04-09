using IdentityService.Application.Interfaces.Messaging;
using IdentityService.Application.Interfaces.Repositories;
using IdentityService.Infrastructure.Messaging;
using IdentityService.Infrastructure.Persistence.DbContexts;
using IdentityService.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<IdentityDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("HrDatabase")));

        services.AddScoped<IUserAccountRepository, UserAccountRepository>();

        services.AddSingleton<IIntegrationEventPublisher, RabbitMqEventPublisher>();

        return services;
    }
}