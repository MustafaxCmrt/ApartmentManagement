using ApartmentManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApartmentManagement.Persistence.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> b)
    {
        b.ToTable("Tenants");
        b.HasKey(x => x.Id);

        b.Property(x => x.Name).IsRequired().HasMaxLength(200);
        b.Property(x => x.ShortName).IsRequired().HasMaxLength(50);
        b.HasIndex(x => x.ShortName).IsUnique();

        b.Property(x => x.ContactEmail).IsRequired().HasMaxLength(200);
        b.Property(x => x.ContactPhone).HasMaxLength(30);
        b.Property(x => x.Address).HasMaxLength(500);
        b.Property(x => x.LogoUrl).HasMaxLength(500);

        b.Property(x => x.IsActive).IsRequired();
        b.Property(x => x.SubscriptionStart).IsRequired();
        b.Property(x => x.MaxApartmentCount).IsRequired();
        b.Property(x => x.CreatedAt).IsRequired();
    }
}
