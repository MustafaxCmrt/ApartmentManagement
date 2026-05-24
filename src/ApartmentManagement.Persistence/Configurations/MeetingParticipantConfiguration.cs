using ApartmentManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApartmentManagement.Persistence.Configurations;

public class MeetingParticipantConfiguration : IEntityTypeConfiguration<MeetingParticipant>
{
    public void Configure(EntityTypeBuilder<MeetingParticipant> b)
    {
        b.ToTable("MeetingParticipants");
        b.HasKey(x => x.Id);

        b.Property(x => x.TenantId).IsRequired();
        b.Property(x => x.MeetingId).IsRequired();
        b.Property(x => x.ApartmentId).IsRequired();
        b.Property(x => x.AttendanceStatus).IsRequired();
        b.Property(x => x.ProxyApartmentId);

        b.HasIndex(x => new { x.MeetingId, x.ApartmentId }).IsUnique();

        b.HasOne(x => x.Apartment)
            .WithMany()
            .HasForeignKey(x => x.ApartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.ProxyApartment)
            .WithMany()
            .HasForeignKey(x => x.ProxyApartmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
