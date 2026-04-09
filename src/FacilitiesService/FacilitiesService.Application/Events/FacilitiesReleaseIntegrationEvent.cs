namespace FacilitiesService.Application.Events;
public class FacilitiesReleaseIntegrationEvent
{
    public Guid OnboardingProcessId { get; set; }
    public int EmployeeId { get; set; }
    public DateTime OccurredAtUtc { get; set; }
}