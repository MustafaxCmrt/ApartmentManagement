using ApartmentManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApartmentManagement.Persistence.Configurations;

public class DuePaymentConfiguration : IEntityTypeConfiguration<DuePayment>
{
    public void Configure(EntityTypeBuilder<DuePayment> b)
    {
        b.ToTable("DuePayments");
        b.HasKey(x => x.Id);

        b.Property(x => x.TenantId).IsRequired();
        b.Property(x => x.DueId).IsRequired();
        b.Property(x => x.PaidAmount).HasColumnType("decimal(18,2)").IsRequired();
        b.Property(x => x.PaymentDate).IsRequired();
        b.Property(x => x.PaymentMethod).IsRequired();
        b.Property(x => x.Description).HasMaxLength(500);
        b.Property(x => x.ReceiptNumber).HasMaxLength(50);

        b.HasIndex(x => new { x.TenantId, x.DueId });
        b.HasIndex(x => new { x.TenantId, x.PaymentDate });
    }
}
