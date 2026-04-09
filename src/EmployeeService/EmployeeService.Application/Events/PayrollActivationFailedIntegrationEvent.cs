namespace EmployeeService.Application.Events;
public class PayrollActivationFailedIntegrationEvent
{
    public Guid OnboardingProcessId { get; set; }
    public int EmployeeId { get; set; }
    public string Reason { get; set; } = null!;
    public DateTime OccurredAtUtc { get; set; }
}