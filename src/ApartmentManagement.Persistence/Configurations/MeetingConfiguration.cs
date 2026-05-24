using ApartmentManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApartmentManagement.Persistence.Configurations;

public class MeetingConfiguration : IEntityTypeConfiguration<Meeting>
{
    public void Configure(EntityTypeBuilder<Meeting> b)
    {
        b.ToTable("Meetings");
        b.HasKey(x => x.Id);

        b.Property(x => x.TenantId).IsRequired();
        b.Property(x => x.Title).IsRequired().HasMaxLength(200);
        b.Property(x => x.MeetingDate).IsRequired();
        b.Property(x => x.Venue).IsRequired().HasMaxLength(200);
        b.Property(x => x.Agenda).IsRequired().HasColumnType("text");
        b.Property(x => x.Status).IsRequired();
        b.Property(x => x.MinutesSummary).HasColumnType("text");

        b.HasIndex(x => new { x.TenantId, x.MeetingDate });

        b.HasMany(x => x.Participants)
            .WithOne(x => x.Meeting)
            .HasForeignKey(x => x.MeetingId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasMany(x => x.Decisions)
            .WithOne(x => x.Meeting)
            .HasForeignKey(x => x.MeetingId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
