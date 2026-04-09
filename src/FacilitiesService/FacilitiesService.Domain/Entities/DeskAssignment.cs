namespace FacilitiesService.Domain.Entities;

public class DeskAssignment
{
    public int DeskAssignmentId { get; set; }
    public int EmployeeId { get; set; }
    public string DeskCode { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime AssignedAtUtc { get; set; }
    public DateTime? ReleasedAtUtc { get; set; }
}