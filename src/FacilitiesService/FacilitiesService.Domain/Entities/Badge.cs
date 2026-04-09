namespace FacilitiesService.Domain.Entities;

public class Badge
{
    public int BadgeId { get; set; }
    public int EmployeeId { get; set; }
    public string BadgeNumber { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime IssuedAtUtc { get; set; }
    public DateTime? RevokedAtUtc { get; set; }
}