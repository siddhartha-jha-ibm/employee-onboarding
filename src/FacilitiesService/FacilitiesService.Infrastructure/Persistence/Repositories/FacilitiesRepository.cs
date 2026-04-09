using FacilitiesService.Application.Interfaces.Repositories;
using FacilitiesService.Domain.Entities;
using FacilitiesService.Infrastructure.Persistence.DbContexts;

namespace FacilitiesService.Infrastructure.Persistence.Repositories;

public class FacilitiesRepository : IFacilitiesRepository
{
    private readonly FacilitiesDbContext _context;

    public FacilitiesRepository(FacilitiesDbContext context)
    {
        _context = context;
    }

    public async Task AssignDeskAsync(DeskAssignment assignment)
    {
        _context.DeskAssignments.Add(assignment);
        await _context.SaveChangesAsync();
    }

    public async Task IssueBadgeAsync(Badge badge)
    {
        _context.Badges.Add(badge);
        await _context.SaveChangesAsync();
    }

    public async Task ReleaseDeskAsync(int employeeId)
    {
        var desk = _context.DeskAssignments
            .FirstOrDefault(d => d.EmployeeId == employeeId && d.IsActive);

        if (desk != null)
        {
            desk.IsActive = false;
            desk.ReleasedAtUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task RevokeBadgeAsync(int employeeId)
    {
        var badge = _context.Badges
            .FirstOrDefault(b => b.EmployeeId == employeeId && b.IsActive);

        if (badge != null)
        {
            badge.IsActive = false;
            badge.RevokedAtUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

}