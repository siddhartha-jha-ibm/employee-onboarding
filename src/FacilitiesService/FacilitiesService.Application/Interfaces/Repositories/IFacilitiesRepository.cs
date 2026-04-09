using FacilitiesService.Domain.Entities;

namespace FacilitiesService.Application.Interfaces.Repositories;

public interface IFacilitiesRepository
{
    Task AssignDeskAsync(DeskAssignment assignment);
    Task IssueBadgeAsync(Badge badge);
    Task ReleaseDeskAsync(int employeeId);
    Task RevokeBadgeAsync(int employeeId);
}