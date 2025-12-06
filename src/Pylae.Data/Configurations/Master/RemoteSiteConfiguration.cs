using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pylae.Data.Entities.Master;

namespace Pylae.Data.Configurations.Master;

public class RemoteSiteConfiguration : IEntityTypeConfiguration<RemoteSite>
{
    public void Configure(EntityTypeBuilder<RemoteSite> builder)
    {
        builder.ToTable("RemoteSites");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.SiteCode)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.DisplayName)
            .HasMaxLength(200);

        builder.Property(x => x.Host)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.Port)
            .IsRequired();

        builder.Property(x => x.ApiKey)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .IsRequired();

        builder.HasIndex(x => x.SiteCode)
            .IsUnique();
    }
}
