using IdentityService.Domain.Entities;

namespace IdentityService.Application.Interfaces.Repositories;

public interface IUserAccountRepository
{
    Task<UserAccount> CreateAsync(UserAccount account);
    Task<UserAccount?> GetByEmployeeIdAsync(int employeeId);
    Task UpdateAsync(UserAccount account);
}