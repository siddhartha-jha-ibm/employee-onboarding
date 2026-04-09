namespace EmployeeService.Application.Events;

public class FacilitiesAllocateIntegrationEvent
{
    public Guid OnboardingProcessId { get; set; }
    public int EmployeeId { get; set; }
    public DateTime OccurredAtUtc { get; set; }
}