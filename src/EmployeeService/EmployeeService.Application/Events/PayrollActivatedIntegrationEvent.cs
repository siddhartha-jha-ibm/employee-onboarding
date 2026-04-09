namespace EmployeeService.Application.Events;

public class PayrollActivatedIntegrationEvent
{
    public Guid OnboardingProcessId { get; set; }
    public int EmployeeId { get; set; }
    public int PayrollAccountId { get; set; }
    public DateTime OccurredAtUtc { get; set; }
}