using Microsoft.EntityFrameworkCore;
using Pylae.Data.Entities.Visits;

namespace Pylae.Data.Context;

public class PylaeVisitsDbContext : DbContext
{
    public PylaeVisitsDbContext(DbContextOptions<PylaeVisitsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Visit> Visits => Set<Visit>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Only apply configurations from the Visits namespace
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(PylaeVisitsDbContext).Assembly,
            t => t.Namespace?.Contains("Configurations.Visits") == true);
    }
}
