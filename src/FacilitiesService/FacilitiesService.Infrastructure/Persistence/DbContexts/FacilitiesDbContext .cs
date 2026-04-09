using FacilitiesService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FacilitiesService.Infrastructure.Persistence.DbContexts;

public class FacilitiesDbContext : DbContext
{
    public FacilitiesDbContext(DbContextOptions<FacilitiesDbContext> options)
        : base(options)
    {
    }

    public DbSet<DeskAssignment> DeskAssignments => Set<DeskAssignment>();
    public DbSet<Badge> Badges => Set<Badge>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DeskAssignment>().ToTable("DeskAssignments");
        modelBuilder.Entity<Badge>().ToTable("Badges");
    }
}