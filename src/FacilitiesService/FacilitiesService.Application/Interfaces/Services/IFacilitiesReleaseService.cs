using FacilitiesService.Application.Events;

namespace FacilitiesService.Application.Interfaces.Services;

public interface IFacilitiesReleaseService
{
    Task ReleaseAsync(FacilitiesReleaseIntegrationEvent @event);
}