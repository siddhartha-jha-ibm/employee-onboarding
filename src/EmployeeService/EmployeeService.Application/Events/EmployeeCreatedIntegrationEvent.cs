namespace EmployeeService.Application.Events;

public class EmployeeCreatedIntegrationEvent
{
    public Guid OnboardingProcessId { get; set; }
    public int EmployeeId { get; set; }

    public string Email { get; set; } = null!;
    public string Department { get; set; } = null!;
    public DateTime OccurredAtUtc { get; set; }
}