namespace EmployeeService.Application.Events;

public class FacilitiesReleasedIntegrationEvent
{
    public Guid OnboardingProcessId { get; set; }
    public int EmployeeId { get; set; }
    public DateTime OccurredAtUtc { get; set; }
}