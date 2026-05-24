using ApartmentManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApartmentManagement.Persistence.Configurations;

public class AnnouncementConfiguration : IEntityTypeConfiguration<Announcement>
{
    public void Configure(EntityTypeBuilder<Announcement> b)
    {
        b.ToTable("Announcements");
        b.HasKey(x => x.Id);

        b.Property(x => x.TenantId).IsRequired();
        b.Property(x => x.Title).IsRequired().HasMaxLength(200);
        b.Property(x => x.Content).IsRequired().HasColumnType("text");
        b.Property(x => x.Severity).IsRequired();
        b.Property(x => x.PublishedAt).IsRequired();
        b.Property(x => x.Audience).IsRequired();
        b.Property(x => x.BuildingId);

        b.HasIndex(x => new { x.TenantId, x.PublishedAt });

        b.HasOne(x => x.Building)
            .WithMany()
            .HasForeignKey(x => x.BuildingId)
            .OnDelete(DeleteBehavior.SetNull);

        b.HasMany(x => x.Reads)
            .WithOne(x => x.Announcement)
            .HasForeignKey(x => x.AnnouncementId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
