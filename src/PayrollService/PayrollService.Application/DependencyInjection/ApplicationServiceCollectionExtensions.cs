using Microsoft.Extensions.DependencyInjection;
using PayrollService.Application.Interfaces.Services;
using PayrollService.Application.Services;

namespace PayrollService.Application.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IPayrollActivationService, PayrollActivationService>();
        return services;
    }
}