namespace PayrollService.Domain.Entities;

public class PayrollAccount
{
    public int PayrollAccountId { get; set; }
    public int EmployeeId { get; set; }
    public decimal SalaryAmount { get; set; }
    public string Currency { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime ActivatedAtUtc { get; set; }
    public DateTime? DeactivatedAtUtc { get; set; }
}