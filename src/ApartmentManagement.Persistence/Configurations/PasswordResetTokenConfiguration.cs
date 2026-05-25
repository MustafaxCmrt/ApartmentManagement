using ApartmentManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApartmentManagement.Persistence.Configurations;

public class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
{
    public void Configure(EntityTypeBuilder<PasswordResetToken> b)
    {
        b.ToTable("PasswordResetTokens");
        b.HasKey(x => x.Id);

        b.Property(x => x.UserId).IsRequired();
        b.Property(x => x.TokenHash).IsRequired().HasMaxLength(200);
        b.Property(x => x.ExpiresAt).IsRequired();
        b.Property(x => x.CreatedAt).IsRequired();
        b.Property(x => x.IsUsed).IsRequired();
        b.Property(x => x.UsedAt);

        b.HasIndex(x => x.TokenHash).IsUnique();
        b.HasIndex(x => new { x.UserId, x.IsUsed });

        b.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasQueryFilter(t => !t.User!.IsDeleted);
    }
}
