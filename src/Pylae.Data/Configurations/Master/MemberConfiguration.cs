using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pylae.Data.Entities.Master;

namespace Pylae.Data.Configurations.Master;

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("Members");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.MemberNumber)
            .IsRequired();

        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.BusinessRank)
            .HasMaxLength(200);

        builder.Property(x => x.Office)
            .HasMaxLength(200);

        builder.Property(x => x.PersonalIdNumber)
            .HasMaxLength(200);

        builder.Property(x => x.BusinessIdNumber)
            .HasMaxLength(200);

        builder.Property(x => x.PhotoFileName)
            .HasMaxLength(200);

        builder.Property(x => x.Phone)
            .HasMaxLength(100);

        builder.Property(x => x.Email)
            .HasMaxLength(256);

        builder.Property(x => x.Notes)
            .HasMaxLength(2000);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(x => x.MemberNumber);
        builder.HasIndex(x => x.LastName);

        builder.HasOne(x => x.MemberType)
            .WithMany()
            .HasForeignKey(x => x.MemberTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
