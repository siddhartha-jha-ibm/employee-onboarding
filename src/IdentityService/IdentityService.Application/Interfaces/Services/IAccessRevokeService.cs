using IdentityService.Application.Events;

namespace IdentityService.Application.Interfaces.Services;

public interface IAccessRevokeService
{
    Task RevokeAsync(AccessRevokeIntegrationEvent @event);
}