namespace EmployeeService.Domain.Entities;

public class OnboardingEvent
{
    public int EventId { get; set; }
    public Guid OnboardingProcessId { get; set; }
    public string EventType { get; set; } = null!;
    public string? EventPayload { get; set; }
    public DateTime OccurredAtUtc { get; set; }
}