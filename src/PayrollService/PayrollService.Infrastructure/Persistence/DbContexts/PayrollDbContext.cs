using Microsoft.EntityFrameworkCore;
using PayrollService.Domain.Entities;

namespace PayrollService.Infrastructure.Persistence.DbContexts;

public class PayrollDbContext : DbContext
{
    public PayrollDbContext(DbContextOptions<PayrollDbContext> options)
        : base(options) { }

    public DbSet<PayrollAccount> PayrollAccounts => Set<PayrollAccount>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PayrollAccount>().ToTable("PayrollAccounts");
    }
}
