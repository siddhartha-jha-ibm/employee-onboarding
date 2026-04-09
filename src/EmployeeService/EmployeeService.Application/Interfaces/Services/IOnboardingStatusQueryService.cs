using EmployeeService.Application.Contracts;

namespace EmployeeService.Application.Interfaces.Services;

public interface IOnboardingStatusQueryService
{
    Task<OnboardingStatusResponse?> GetStatusAsync(Guid onboardingProcessId);
}