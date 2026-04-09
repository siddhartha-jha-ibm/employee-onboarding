using IdentityService.Application.Events;
using IdentityService.Application.Interfaces.Messaging;
using IdentityService.Application.Interfaces.Repositories;
using IdentityService.Application.Interfaces.Services;

namespace IdentityService.Application.Services;

public class AccessRevokeService : IAccessRevokeService
{
    private readonly IUserAccountRepository _repository;
    private readonly IIntegrationEventPublisher _publisher;

    public AccessRevokeService(
        IUserAccountRepository repository,
        IIntegrationEventPublisher publisher)
    {
        _repository = repository;
        _publisher = publisher;
    }

    public async Task RevokeAsync(AccessRevokeIntegrationEvent @event)
    {
        var account = await _repository.GetByEmployeeIdAsync(@event.EmployeeId);

        if (account != null && account.IsActive)
        {
            account.IsActive = false;
            account.DisabledAtUtc = DateTime.UtcNow;
            await _repository.UpdateAsync(account);
        }

        await _publisher.PublishAsync(
            new
            {
                @event.OnboardingProcessId,
                @event.EmployeeId,
                OccurredAtUtc = DateTime.UtcNow
            },
            eventName: "access.revoked");
    }
}