using ApartmentManagement.Domain.Common;

namespace ApartmentManagement.Domain.Entities;

public class Building : BaseEntity, ITenantEntity, IAuditableEntity, ISoftDeletable
{
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int FloorCount { get; set; }
    public int ApartmentCount { get; set; }
    public int? ConstructionYear { get; set; }

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
    public ICollection<Apartment> Apartments { get; set; } = new List<Apartment>();
}
