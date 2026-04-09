using PayrollService.Application.Interfaces.Repositories;
using PayrollService.Domain.Entities;
using PayrollService.Infrastructure.Persistence.DbContexts;

namespace PayrollService.Infrastructure.Persistence.Repositories;

public class PayrollRepository : IPayrollRepository
{
    private readonly PayrollDbContext _context;

    public PayrollRepository(PayrollDbContext context)
    {
        _context = context;
    }

    public async Task<PayrollAccount> CreateAsync(PayrollAccount account)
    {
        _context.PayrollAccounts.Add(account);
        await _context.SaveChangesAsync();
        return account;
    }
}