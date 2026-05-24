using ApartmentManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApartmentManagement.Persistence.Configurations;

public class ApartmentConfiguration : IEntityTypeConfiguration<Apartment>
{
    public void Configure(EntityTypeBuilder<Apartment> b)
    {
        b.ToTable("Apartments");
        b.HasKey(x => x.Id);

        b.Property(x => x.TenantId).IsRequired();
        b.Property(x => x.BuildingId).IsRequired();
        b.Property(x => x.ApartmentNumber).IsRequired().HasMaxLength(20);
        b.Property(x => x.Floor).IsRequired();
        b.Property(x => x.GrossSquareMeters).HasColumnType("decimal(10,2)");
        b.Property(x => x.NetSquareMeters).HasColumnType("decimal(10,2)");
        b.Property(x => x.OccupancyStatus).IsRequired();
        b.Property(x => x.DueMultiplier)
            .HasColumnType("decimal(10,4)")
            .HasDefaultValue(1.0m)
            .IsRequired();

        b.HasIndex(x => new { x.TenantId, x.BuildingId, x.ApartmentNumber }).IsUnique();
        b.HasIndex(x => x.TenantId);

        b.HasMany(x => x.Residents)
            .WithOne(x => x.Apartment)
            .HasForeignKey(x => x.ApartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasMany(x => x.Dues)
            .WithOne(x => x.Apartment)
            .HasForeignKey(x => x.ApartmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
