using WebApi_JobService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobService.Infrastructure.Persistence;

public class JobServiceDbContext : DbContext
{
    public JobServiceDbContext(DbContextOptions<JobServiceDbContext> options)
        : base(options)
    {
    }

    public DbSet<Job> Jobs { get; set; }
    public DbSet<JobDescriptionAndNote> JobDescriptionAndNotes { get; set; }
    public DbSet<JobDoc> JobDocs { get; set; }
    public DbSet<JobStock> JobStocks { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(JobServiceDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}