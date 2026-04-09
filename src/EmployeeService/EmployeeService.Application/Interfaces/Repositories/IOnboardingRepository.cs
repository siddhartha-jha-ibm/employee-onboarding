using EmployeeService.Domain.Entities;

namespace EmployeeService.Application.Interfaces.Repositories;

public interface IOnboardingRepository
{
    Task<OnboardingProcess> AddAsync(OnboardingProcess process);
    Task<OnboardingProcess?> GetByIdAsync(Guid processId);
    Task UpdateAsync(OnboardingProcess process);
    Task AddEventAsync(OnboardingEvent onboardingEvent);
    Task<(OnboardingProcess Process, Employee Employee)?> GetProcessWithEmployeeAsync(Guid onboardingProcessId);
}
