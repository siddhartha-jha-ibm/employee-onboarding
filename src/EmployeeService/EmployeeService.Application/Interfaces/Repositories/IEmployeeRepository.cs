using EmployeeService.Domain.Entities;

namespace EmployeeService.Application.Interfaces.Repositories;

public interface IEmployeeRepository
{
    Task<Employee> AddAsync(Employee employee);
    Task<Employee?> GetByIdAsync(int employeeId);
}
