using ApartmentManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApartmentManagement.Persistence.Configurations;

public class MaintenanceCommentConfiguration : IEntityTypeConfiguration<MaintenanceComment>
{
    public void Configure(EntityTypeBuilder<MaintenanceComment> b)
    {
        b.ToTable("MaintenanceComments");
        b.HasKey(x => x.Id);

        b.Property(x => x.TenantId).IsRequired();
        b.Property(x => x.MaintenanceTicketId).IsRequired();
        b.Property(x => x.UserId).IsRequired();
        b.Property(x => x.Comment).IsRequired().HasColumnType("text");

        b.HasIndex(x => new { x.TenantId, x.MaintenanceTicketId });

        b.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
