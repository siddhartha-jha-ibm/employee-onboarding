using EmployeeService.Application.Contracts;

namespace EmployeeService.Application.Interfaces.Services;

public interface IEmployeeOnboardingService
{
    Task<EmployeeOnboardingResult> StartOnboardingAsync(CreateEmployeeRequest request);
}
