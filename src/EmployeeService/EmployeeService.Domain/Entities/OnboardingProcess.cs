namespace EmployeeService.Domain.Entities;

public class OnboardingProcess
{
    public Guid OnboardingProcessId { get; set; }
    public int EmployeeId { get; set; }
    public string Status { get; set; } = null!;
    public string? CurrentStep { get; set; }
    public string? FailureReason { get; set; }

    public bool IsAccessProvisioned { get; set; }
    public bool IsFacilitiesAssigned { get; set; }
    public bool IsPayrollActivated { get; set; }

    public DateTime StartedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
}