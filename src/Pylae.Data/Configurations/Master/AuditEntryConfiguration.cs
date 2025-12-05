using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pylae.Data.Entities.Master;

namespace Pylae.Data.Configurations.Master;

public class AuditEntryConfiguration : IEntityTypeConfiguration<AuditEntry>
{
    public void Configure(EntityTypeBuilder<AuditEntry> builder)
    {
        builder.ToTable("AuditLog");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TimestampUtc)
            .IsRequired();

        builder.Property(x => x.SiteCode)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Username)
            .HasMaxLength(150);

        builder.Property(x => x.ActionType)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.TargetType)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.TargetId)
            .HasMaxLength(200);

        builder.Property(x => x.DetailsJson)
            .HasMaxLength(4000);

        builder.HasIndex(x => x.TimestampUtc);
        builder.HasIndex(x => x.ActionType);
    }
}
