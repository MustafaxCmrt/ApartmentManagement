using ApartmentManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApartmentManagement.Persistence.Configurations;

public class DecisionConfiguration : IEntityTypeConfiguration<Decision>
{
    public void Configure(EntityTypeBuilder<Decision> b)
    {
        b.ToTable("Decisions");
        b.HasKey(x => x.Id);

        b.Property(x => x.TenantId).IsRequired();
        b.Property(x => x.MeetingId);
        b.Property(x => x.DecisionNumber).IsRequired();
        b.Property(x => x.DecisionDate).IsRequired();
        b.Property(x => x.DecisionTitle).IsRequired().HasMaxLength(300);
        b.Property(x => x.DecisionText).IsRequired().HasColumnType("text");

        b.HasIndex(x => new { x.TenantId, x.DecisionNumber }).IsUnique();
        b.HasIndex(x => new { x.TenantId, x.DecisionDate });
    }
}
