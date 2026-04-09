using EmployeeService.Application.Interfaces.Services;
using EmployeeService.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeService.Application.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IEmployeeOnboardingService, EmployeeOnboardingService>();
        services.AddScoped<IOnboardingStatusQueryService, OnboardingStatusQueryService>();
        services.AddScoped<IOnboardingSagaService, OnboardingSagaService>();
        return services;
    }
}