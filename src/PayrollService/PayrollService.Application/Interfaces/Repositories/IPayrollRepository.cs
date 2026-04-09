using PayrollService.Domain.Entities;

namespace PayrollService.Application.Interfaces.Repositories;

public interface IPayrollRepository
{
    Task<PayrollAccount> CreateAsync(PayrollAccount account);
}
