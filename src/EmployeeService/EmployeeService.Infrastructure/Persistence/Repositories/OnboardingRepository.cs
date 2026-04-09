using EmployeeService.Application.Interfaces.Repositories;
using EmployeeService.Domain.Entities;
using EmployeeService.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace EmployeeService.Infrastructure.Persistence.Repositories;

public class OnboardingRepository : IOnboardingRepository
{
    private readonly EmployeeDbContext _context;

    public OnboardingRepository(EmployeeDbContext context)
    {
        _context = context;
    }

    public async Task<OnboardingProcess> AddAsync(OnboardingProcess process)
    {
        _context.OnboardingProcesses.Add(process);
        await _context.SaveChangesAsync();
        return process;
    }

    public async Task<OnboardingProcess?> GetByIdAsync(Guid processId)
    {
        return await _context.OnboardingProcesses
            .FirstOrDefaultAsync(p => p.OnboardingProcessId == processId);
    }

    public async Task UpdateAsync(OnboardingProcess process)
    {
        _context.OnboardingProcesses.Update(process);
        await _context.SaveChangesAsync();
    }

    public async Task AddEventAsync(OnboardingEvent onboardingEvent)
    {
        _context.OnboardingEvents.Add(onboardingEvent);
        await _context.SaveChangesAsync();
    }


    public async Task<(OnboardingProcess Process, Employee Employee)?>
           GetProcessWithEmployeeAsync(Guid onboardingProcessId)
    {
        var result =
            from process in _context.OnboardingProcesses
            join employee in _context.Employees
                on process.EmployeeId equals employee.EmployeeId
            where process.OnboardingProcessId == onboardingProcessId
            select new { process, employee };

        var data = await result.FirstOrDefaultAsync();

        if (data == null)
            return null;

        return (data.process, data.employee);
    }

}