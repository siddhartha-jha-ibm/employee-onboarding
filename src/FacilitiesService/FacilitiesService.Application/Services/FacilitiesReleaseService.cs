using FacilitiesService.Application.Events;
using FacilitiesService.Application.Interfaces.Messaging;
using FacilitiesService.Application.Interfaces.Repositories;
using FacilitiesService.Application.Interfaces.Services;

namespace FacilitiesService.Application.Services;

public class FacilitiesReleaseService : IFacilitiesReleaseService
{
    private readonly IFacilitiesRepository _repository;
    private readonly IIntegrationEventPublisher _publisher;

    public FacilitiesReleaseService(
        IFacilitiesRepository repository,
        IIntegrationEventPublisher publisher)
    {
        _repository = repository;
        _publisher = publisher;
    }

    public async Task ReleaseAsync(FacilitiesReleaseIntegrationEvent @event)
    {
        await _repository.ReleaseDeskAsync(@event.EmployeeId);
        await _repository.RevokeBadgeAsync(@event.EmployeeId);

        await _publisher.PublishAsync(
            new
            {
                @event.OnboardingProcessId,
                @event.EmployeeId,
                OccurredAtUtc = DateTime.UtcNow
            },
            eventName: "facilities.released");
    }
}
