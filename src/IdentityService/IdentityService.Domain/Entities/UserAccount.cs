namespace IdentityService.Domain.Entities;

public class UserAccount
{
    public int UserAccountId { get; set; }
    public int EmployeeId { get; set; }
    public string Username { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? DisabledAtUtc { get; set; }
}