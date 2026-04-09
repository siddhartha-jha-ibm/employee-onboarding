using FacilitiesService.Application.Interfaces.Messaging;
using FacilitiesService.Application.Interfaces.Repositories;
using FacilitiesService.Infrastructure.Messaging;
using FacilitiesService.Infrastructure.Persistence.DbContexts;
using FacilitiesService.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FacilitiesService.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<FacilitiesDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("HrDatabase")));

        services.AddScoped<IFacilitiesRepository, FacilitiesRepository>();

        services.AddSingleton<IIntegrationEventPublisher, RabbitMqEventPublisher>();

        return services;
    }
}