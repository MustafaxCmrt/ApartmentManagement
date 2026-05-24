using ApartmentManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApartmentManagement.Persistence.Configurations;

public class DueConfiguration : IEntityTypeConfiguration<Due>
{
    public void Configure(EntityTypeBuilder<Due> b)
    {
        b.ToTable("Dues");
        b.HasKey(x => x.Id);

        b.Property(x => x.TenantId).IsRequired();
        b.Property(x => x.ApartmentId).IsRequired();
        b.Property(x => x.Period).IsRequired();
        b.Property(x => x.Amount).HasColumnType("decimal(18,2)").IsRequired();
        b.Property(x => x.DueDate).IsRequired();
        b.Property(x => x.Description).HasMaxLength(500);
        b.Property(x => x.Status).IsRequired();
        b.Property(x => x.CreationType).IsRequired();

        b.HasIndex(x => new { x.TenantId, x.ApartmentId, x.Period }).IsUnique();
        b.HasIndex(x => new { x.TenantId, x.Status, x.DueDate });
        b.HasIndex(x => new { x.TenantId, x.Period });

        b.HasMany(x => x.Payments)
            .WithOne(x => x.Due)
            .HasForeignKey(x => x.DueId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
