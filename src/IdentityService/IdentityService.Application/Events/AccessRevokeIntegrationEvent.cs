namespace IdentityService.Application.Events;

public class AccessRevokeIntegrationEvent
{
    public Guid OnboardingProcessId { get; set; }
    public int EmployeeId { get; set; }
    public DateTime OccurredAtUtc { get; set; }
}