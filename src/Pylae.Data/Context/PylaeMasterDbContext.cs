using Microsoft.EntityFrameworkCore;
using Pylae.Data.Entities.Master;

namespace Pylae.Data.Context;

public class PylaeMasterDbContext : DbContext
{
    public PylaeMasterDbContext(DbContextOptions<PylaeMasterDbContext> options)
        : base(options)
    {
    }

    public DbSet<Member> Members => Set<Member>();

    public DbSet<MemberType> MemberTypes => Set<MemberType>();

    public DbSet<User> Users => Set<User>();

    public DbSet<Setting> Settings => Set<Setting>();

    public DbSet<AuditEntry> AuditEntries => Set<AuditEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PylaeMasterDbContext).Assembly);
    }
}
