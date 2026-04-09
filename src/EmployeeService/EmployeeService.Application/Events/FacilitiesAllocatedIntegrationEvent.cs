namespace EmployeeService.Application.Events;

public class FacilitiesAllocatedIntegrationEvent
{
    public Guid OnboardingProcessId { get; set; }
    public int EmployeeId { get; set; }
    public string DeskCode { get; set; } = null!;
    public string BadgeNumber { get; set; } = null!;
    public DateTime OccurredAtUtc { get; set; }
}