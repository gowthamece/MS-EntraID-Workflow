using Microsoft.EntityFrameworkCore;
using EntraID.Workflow.ApiService.Models;

namespace EntraID.Workflow.ApiService.DBContext;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Workflow.ApiService.Models.Workflow> Workflows { get; set; }
    public DbSet<AppRegistration> AppRegistrations { get; set; }
    public DbSet<AppType> AppTypes { get; set; }
    public DbSet<Status> Statuses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Workflow.ApiService.Models.Workflow>().ToTable("WorkFlow");
        modelBuilder.Entity<Workflow.ApiService.Models.AppType>().ToTable("AppType");
        modelBuilder.Entity<Workflow.ApiService.Models.Status>().ToTable("Status");
        modelBuilder.Entity<Workflow.ApiService.Models.AppRegistration>().ToTable("AppRegistration");

        modelBuilder.Entity<AppType>().HasData(
            new AppType(1, "Web"),
            new AppType(2, "SPA")
        );
        modelBuilder.Entity<AppRegistration>()
            .HasOne(ar => ar.AppType)
            .WithMany()
            .HasForeignKey(ar => ar.AppTypeId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Status>().HasData(
            new Status { Id = 1, Name = "Submitted" },
            new Status { Id = 2, Name = "In progress" },
            new Status { Id = 3, Name = "Completed" }
        );
    }
}
