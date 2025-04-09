using Microsoft.EntityFrameworkCore;


namespace EntraID.Workflow.ApiService.DBContext;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Workflow.ApiService.Models.Workflow> Workflows { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Workflow.ApiService.Models.Workflow>().ToTable("WorkFlow");
    }
}
