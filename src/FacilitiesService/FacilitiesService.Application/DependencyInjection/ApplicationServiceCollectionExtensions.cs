using FacilitiesService.Application.Interfaces.Services;
using FacilitiesService.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FacilitiesService.Application.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IFacilitiesAllocationService, FacilitiesAllocationService>();
        return services;
    }
}