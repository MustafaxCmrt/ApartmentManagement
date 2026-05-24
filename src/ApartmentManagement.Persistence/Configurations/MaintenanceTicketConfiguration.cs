using ApartmentManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApartmentManagement.Persistence.Configurations;

public class MaintenanceTicketConfiguration : IEntityTypeConfiguration<MaintenanceTicket>
{
    public void Configure(EntityTypeBuilder<MaintenanceTicket> b)
    {
        b.ToTable("MaintenanceTickets");
        b.HasKey(x => x.Id);

        b.Property(x => x.TenantId).IsRequired();
        b.Property(x => x.ApartmentId);
        b.Property(x => x.RequestedByUserId).IsRequired();
        b.Property(x => x.Title).IsRequired().HasMaxLength(200);
        b.Property(x => x.Description).IsRequired().HasColumnType("text");
        b.Property(x => x.Location).IsRequired().HasMaxLength(200);
        b.Property(x => x.Priority).IsRequired();
        b.Property(x => x.Status).IsRequired();
        b.Property(x => x.AssignedTo).HasMaxLength(200);
        b.Property(x => x.EstimatedCost).HasColumnType("decimal(18,2)");
        b.Property(x => x.ActualCost).HasColumnType("decimal(18,2)");

        b.HasIndex(x => new { x.TenantId, x.Status });
        b.HasIndex(x => new { x.TenantId, x.Priority });

        b.HasOne(x => x.Apartment)
            .WithMany()
            .HasForeignKey(x => x.ApartmentId)
            .OnDelete(DeleteBehavior.SetNull);

        b.HasOne(x => x.RequestedByUser)
            .WithMany()
            .HasForeignKey(x => x.RequestedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasMany(x => x.Comments)
            .WithOne(x => x.MaintenanceTicket)
            .HasForeignKey(x => x.MaintenanceTicketId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
