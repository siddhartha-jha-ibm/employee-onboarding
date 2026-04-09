using FacilitiesService.Application.Events;

namespace FacilitiesService.Application.Interfaces.Services;

public interface IFacilitiesAllocationService
{
    Task AllocateAsync(FacilitiesAllocateIntegrationEvent @event);
}