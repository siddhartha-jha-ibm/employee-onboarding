using IdentityService.Application.Interfaces.Repositories;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Persistence.Repositories;

public class UserAccountRepository : IUserAccountRepository
{
    private readonly IdentityDbContext _context;

    public UserAccountRepository(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<UserAccount> CreateAsync(UserAccount account)
    {
        _context.UserAccounts.Add(account);
        await _context.SaveChangesAsync();
        return account;
    }

    public async Task<UserAccount?> GetByEmployeeIdAsync(int employeeId)
    {
        return await _context.UserAccounts
            .FirstOrDefaultAsync(u => u.EmployeeId == employeeId);
    }

    public async Task UpdateAsync(UserAccount account)
    {
        _context.UserAccounts.Update(account);
        await _context.SaveChangesAsync();
    }

}