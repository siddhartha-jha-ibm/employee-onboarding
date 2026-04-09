using FacilitiesService.Application.Events;
using FacilitiesService.Application.Interfaces.Messaging;
using FacilitiesService.Application.Interfaces.Repositories;
using FacilitiesService.Application.Interfaces.Services;
using FacilitiesService.Domain.Entities;

namespace FacilitiesService.Application.Services;

public class FacilitiesAllocationService : IFacilitiesAllocationService
{
    private readonly IFacilitiesRepository _repository;
    private readonly IIntegrationEventPublisher _publisher;

    public FacilitiesAllocationService(
        IFacilitiesRepository repository,
        IIntegrationEventPublisher publisher)
    {
        _repository = repository;
        _publisher = publisher;
    }

    public async Task AllocateAsync(FacilitiesAllocateIntegrationEvent @event)
    {
        var deskCode = $"DESK-{Guid.NewGuid():N}".Substring(0, 10);
        var badgeNumber = $"BADGE-{Guid.NewGuid():N}".Substring(0, 12);

        await _repository.AssignDeskAsync(new DeskAssignment
        {
            EmployeeId = @event.EmployeeId,
            DeskCode = deskCode,
            IsActive = true,
            AssignedAtUtc = DateTime.UtcNow
        });

        await _repository.IssueBadgeAsync(new Badge
        {
            EmployeeId = @event.EmployeeId,
            BadgeNumber = badgeNumber,
            IsActive = true,
            IssuedAtUtc = DateTime.UtcNow
        });

        var allocatedEvent = new FacilitiesAllocatedIntegrationEvent
        {
            OnboardingProcessId = @event.OnboardingProcessId,
            EmployeeId = @event.EmployeeId,
            DeskCode = deskCode,
            BadgeNumber = badgeNumber,
            OccurredAtUtc = DateTime.UtcNow
        };

        await _publisher.PublishAsync(
            allocatedEvent,
            eventName: "facilities.allocated");
    }
}