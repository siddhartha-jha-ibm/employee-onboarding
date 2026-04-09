using IdentityService.Application.Events;
using IdentityService.Application.Interfaces.Messaging;
using IdentityService.Application.Interfaces.Repositories;
using IdentityService.Application.Interfaces.Services;
using IdentityService.Domain.Entities;

namespace IdentityService.Application.Services;

public class IdentityProvisioningService : IIdentityProvisioningService
{
    private readonly IUserAccountRepository _repository;
    private readonly IIntegrationEventPublisher _publisher;

    public IdentityProvisioningService(
        IUserAccountRepository repository,
        IIntegrationEventPublisher publisher)
    {
        _repository = repository;
        _publisher = publisher;
    }

    public async Task ProvisionAccessAsync(EmployeeCreatedIntegrationEvent @event)
    {
        // Create user account
        var account = new UserAccount
        {
            EmployeeId = @event.EmployeeId,
            Username = @event.Email,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        account = await _repository.CreateAsync(account);

        // Publish access.provisioned
        var provisionedEvent = new AccessProvisionedIntegrationEvent
        {
            OnboardingProcessId = @event.OnboardingProcessId,
            EmployeeId = @event.EmployeeId,
            UserAccountId = account.UserAccountId,
            OccurredAtUtc = DateTime.UtcNow
        };

        await _publisher.PublishAsync(
            provisionedEvent,
            eventName: "access.provisioned");
    }
}