using ApartmentManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApartmentManagement.Persistence.Configurations;

public class ResidentConfiguration : IEntityTypeConfiguration<Resident>
{
    public void Configure(EntityTypeBuilder<Resident> b)
    {
        b.ToTable("Residents");
        b.HasKey(x => x.Id);

        b.Property(x => x.TenantId).IsRequired();
        b.Property(x => x.ApartmentId).IsRequired();
        b.Property(x => x.UserId);
        b.Property(x => x.FullName).IsRequired().HasMaxLength(200);
        b.Property(x => x.Phone).IsRequired().HasMaxLength(30);
        b.Property(x => x.Email).HasMaxLength(200);
        b.Property(x => x.ResidentType).IsRequired();
        b.Property(x => x.MoveInDate).IsRequired();
        b.Property(x => x.IsPrimaryContact).IsRequired();

        b.HasIndex(x => new { x.TenantId, x.ApartmentId });
        b.HasIndex(x => x.TenantId);

        b.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
