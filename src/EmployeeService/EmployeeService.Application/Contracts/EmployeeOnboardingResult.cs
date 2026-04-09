namespace EmployeeService.Application.Contracts;

public class EmployeeOnboardingResult
{
    public int EmployeeId { get; set; }
    public Guid OnboardingProcessId { get; set; }
    public string Status { get; set; } = null!;
}