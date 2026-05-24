using ApartmentManagement.Domain.Common;
using ApartmentManagement.Domain.Enums;

namespace ApartmentManagement.Domain.Entities;

public class Due : BaseEntity, ITenantEntity, IAuditableEntity, ISoftDeletable
{
    public Guid TenantId { get; set; }
    public Guid ApartmentId { get; set; }
    public DateTime Period { get; set; }
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public string? Description { get; set; }
    public DueStatus Status { get; set; }
    public DueCreationType CreationType { get; set; }

    // IAuditableEntity
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    // ISoftDeletable
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    // Navigation
    public Apartment? Apartment { get; set; }
    public ICollection<DuePayment> Payments { get; set; } = new List<DuePayment>();
}
