using ApartmentManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApartmentManagement.Persistence.Configurations;

public class BuildingConfiguration : IEntityTypeConfiguration<Building>
{
    public void Configure(EntityTypeBuilder<Building> b)
    {
        b.ToTable("Buildings");
        b.HasKey(x => x.Id);

        b.Property(x => x.TenantId).IsRequired();
        b.Property(x => x.Name).IsRequired().HasMaxLength(100);
        b.Property(x => x.Address).IsRequired().HasMaxLength(500);
        b.Property(x => x.FloorCount).IsRequired();
        b.Property(x => x.ApartmentCount).IsRequired();

        b.HasIndex(x => x.TenantId);
        b.HasIndex(x => new { x.TenantId, x.Name });

        b.HasMany(x => x.Apartments)
            .WithOne(x => x.Building)
            .HasForeignKey(x => x.BuildingId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
