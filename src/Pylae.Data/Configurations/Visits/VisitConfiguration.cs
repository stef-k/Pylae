using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pylae.Core.Enums;
using Pylae.Data.Entities.Visits;

namespace Pylae.Data.Configurations.Visits;

public class VisitConfiguration : IEntityTypeConfiguration<Visit>
{
    public void Configure(EntityTypeBuilder<Visit> builder)
    {
        builder.ToTable("Visits");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.MemberId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.MemberFirstName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.MemberLastName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.MemberBusinessRank)
            .HasMaxLength(200);

        builder.Property(x => x.MemberOfficeName)
            .HasMaxLength(200);

        builder.Property(x => x.MemberTypeCode)
            .HasMaxLength(100);

        builder.Property(x => x.MemberTypeName)
            .HasMaxLength(200);

        builder.Property(x => x.MemberPersonalIdNumber)
            .HasMaxLength(200);

        builder.Property(x => x.MemberBusinessIdNumber)
            .HasMaxLength(200);

        builder.Property(x => x.SiteCode)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Username)
            .HasMaxLength(150);

        builder.Property(x => x.UserDisplayName)
            .HasMaxLength(200);

        builder.Property(x => x.WorkstationId)
            .HasMaxLength(200);

        builder.Property(x => x.Notes)
            .HasMaxLength(2000);

        builder.Property(x => x.Direction)
            .HasConversion(BuildDirectionConverter())
            .IsRequired();

        builder.Property(x => x.Method)
            .HasConversion(BuildMethodConverter())
            .IsRequired();

        builder.HasIndex(x => x.TimestampUtc);
        builder.HasIndex(x => new { x.MemberNumber, x.TimestampUtc });
        builder.HasIndex(x => new { x.SiteCode, x.TimestampUtc });
    }

    private static ValueConverter<VisitDirection, string> BuildDirectionConverter()
    {
        return new ValueConverter<VisitDirection, string>(
            v => v.ToString().ToLowerInvariant(),
            v => Enum.Parse<VisitDirection>(v, true));
    }

    private static ValueConverter<VisitMethod, string> BuildMethodConverter()
    {
        return new ValueConverter<VisitMethod, string>(
            v => v.ToString().ToLowerInvariant(),
            v => Enum.Parse<VisitMethod>(v, true));
    }
}
