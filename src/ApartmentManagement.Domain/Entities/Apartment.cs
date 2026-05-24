using ApartmentManagement.Domain.Common;
using ApartmentManagement.Domain.Enums;

namespace ApartmentManagement.Domain.Entities;

public class Apartment : BaseEntity, ITenantEntity, IAuditableEntity, ISoftDeletable
{
    public Guid TenantId { get; set; }
    public Guid BuildingId { get; set; }
    public string ApartmentNumber { get; set; } = string.Empty;
    public int Floor { get; set; }
    public decimal? GrossSquareMeters { get; set; }
    public decimal? NetSquareMeters { get; set; }
    public OccupancyStatus OccupancyStatus { get; set; }
    public decimal DueMultiplier { get; set; } = 1.0m;

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
    public Building? Building { get; set; }
    public ICollection<Resident> Residents { get; set; } = new List<Resident>();
    public ICollection<Due> Dues { get; set; } = new List<Due>();
}
