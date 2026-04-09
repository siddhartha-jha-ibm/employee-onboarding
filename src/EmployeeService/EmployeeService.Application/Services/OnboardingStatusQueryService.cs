using EmployeeService.Application.Contracts;
using EmployeeService.Application.Interfaces.Repositories;
using EmployeeService.Application.Interfaces.Services;

namespace EmployeeService.Application.Services;

public class OnboardingStatusQueryService : IOnboardingStatusQueryService
{
    private readonly IOnboardingRepository _onboardingRepository;

    public OnboardingStatusQueryService(IOnboardingRepository onboardingRepository)
    {
        _onboardingRepository = onboardingRepository;
    }

    public async Task<OnboardingStatusResponse?> GetStatusAsync(Guid onboardingProcessId)
    {
        var result =
            await _onboardingRepository.GetProcessWithEmployeeAsync(onboardingProcessId);

        if (result == null)
            return null;

        var (process, employee) = result.Value;

        return new OnboardingStatusResponse
        {
            OnboardingProcessId = process.OnboardingProcessId,
            EmployeeId = employee.EmployeeId,
            EmployeeName = $"{employee.FirstName} {employee.LastName}",
            Email = employee.Email,

            Status = process.Status,
            CurrentStep = process.CurrentStep,
            FailureReason = process.FailureReason,

            IsAccessProvisioned = process.IsAccessProvisioned,
            IsFacilitiesAssigned = process.IsFacilitiesAssigned,
            IsPayrollActivated = process.IsPayrollActivated,

            StartedAtUtc = process.StartedAtUtc,
            CompletedAtUtc = process.CompletedAtUtc
        };
    }
}