using ApartmentManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApartmentManagement.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> b)
    {
        b.ToTable("Notifications");
        b.HasKey(x => x.Id);

        b.Property(x => x.TenantId).IsRequired();
        b.Property(x => x.UserId).IsRequired();
        b.Property(x => x.Type).IsRequired();
        b.Property(x => x.Title).IsRequired().HasMaxLength(200);
        b.Property(x => x.Message).IsRequired().HasMaxLength(1000);
        b.Property(x => x.Link).HasMaxLength(500);
        b.Property(x => x.IsRead).IsRequired();
        b.Property(x => x.ReadAt);
        b.Property(x => x.CreatedAt).IsRequired();

        b.Property(x => x.IsDeleted).IsRequired();
        b.Property(x => x.DeletedAt);
        b.Property(x => x.DeletedBy);

        b.HasIndex(x => new { x.UserId, x.IsRead });
        b.HasIndex(x => new { x.TenantId, x.UserId, x.CreatedAt });

        b.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
