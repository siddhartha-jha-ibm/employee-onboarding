namespace IdentityService.Domain.Entities;

public class UserRole
{
    public int UserRoleId { get; set; }
    public int UserAccountId { get; set; }
    public string RoleName { get; set; } = null!;
}