using IdentityService.Application.Events;

namespace IdentityService.Application.Interfaces.Services;

public interface IIdentityProvisioningService
{
    Task ProvisionAccessAsync(EmployeeCreatedIntegrationEvent @event);
}
