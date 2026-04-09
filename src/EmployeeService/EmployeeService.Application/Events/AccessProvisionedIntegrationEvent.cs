namespace EmployeeService.Application.Events;

public class AccessProvisionedIntegrationEvent
{
    public Guid OnboardingProcessId { get; set; }
    public int EmployeeId { get; set; }
    public int UserAccountId { get; set; }
    public DateTime OccurredAtUtc { get; set; }
}