using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pylae.Core.Enums;
using Pylae.Data.Entities.Master;

namespace Pylae.Data.Configurations.Master;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Username)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasIndex(x => x.Username)
            .IsUnique();

        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.PasswordHash)
            .HasMaxLength(500);

        builder.Property(x => x.PasswordSalt)
            .HasMaxLength(500);

        builder.Property(x => x.QuickCodeHash)
            .HasMaxLength(500);

        builder.Property(x => x.QuickCodeSalt)
            .HasMaxLength(500);

        builder.Property(x => x.Role)
            .HasConversion(BuildRoleConverter())
            .IsRequired();

        builder.Property(x => x.IsShared)
            .HasDefaultValue(false);

        builder.Property(x => x.IsSystem)
            .HasDefaultValue(false);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();
    }

    private static ValueConverter<UserRole, string> BuildRoleConverter()
    {
        return new ValueConverter<UserRole, string>(
            v => v.ToString().ToLowerInvariant(),
            v => Enum.Parse<UserRole>(v, true));
    }
}
