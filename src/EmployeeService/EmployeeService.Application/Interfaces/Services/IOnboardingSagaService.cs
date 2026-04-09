using EmployeeService.Application.Events;

namespace EmployeeService.Application.Interfaces.Services;

public interface IOnboardingSagaService
{
    Task HandleAccessProvisionedAsync(AccessProvisionedIntegrationEvent @event);
    Task HandleFacilitiesAllocatedAsync(FacilitiesAllocatedIntegrationEvent @event);
    Task HandlePayrollActivatedAsync(PayrollActivatedIntegrationEvent @event);
    Task HandlePayrollActivationFailedAsync(PayrollActivationFailedIntegrationEvent @event);
    Task HandleFacilitiesReleasedAsync(FacilitiesReleasedIntegrationEvent @event);
    Task HandleAccessRevokedAsync(AccessRevokedIntegrationEvent @event);
}