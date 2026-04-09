namespace EmployeeService.Application.Contracts;

public class CreateEmployeeRequest
{
    public string EmployeeCode { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Department { get; set; } = null!;
    public string JobTitle { get; set; } = null!;
    public DateTime HireDate { get; set; }

    public string RequestedBy { get; set; } = null!;
}