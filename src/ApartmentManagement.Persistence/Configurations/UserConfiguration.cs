using ApartmentManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApartmentManagement.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.ToTable("Users");
        b.HasKey(x => x.Id);

        b.Property(x => x.TenantId);
        b.Property(x => x.Email).IsRequired().HasMaxLength(200);
        b.Property(x => x.PasswordHash).IsRequired().HasMaxLength(200);
        b.Property(x => x.FullName).IsRequired().HasMaxLength(200);
        b.Property(x => x.Phone).IsRequired().HasMaxLength(30);
        b.Property(x => x.Role).IsRequired();
        b.Property(x => x.IsActive).IsRequired();
        b.Property(x => x.IsEmailVerified).IsRequired();

        b.HasIndex(x => x.Email).IsUnique();
        b.HasIndex(x => x.Phone).IsUnique();
        b.HasIndex(x => new { x.TenantId, x.Role });
    }
}
