using ApartmentManagement.Domain.Common;
using ApartmentManagement.Domain.Enums;

namespace ApartmentManagement.Domain.Entities;

public class MaintenanceTicket : BaseEntity, ITenantEntity, IAuditableEntity, ISoftDeletable
{
    public Guid TenantId { get; set; }
    public Guid? ApartmentId { get; set; }
    public Guid RequestedByUserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public MaintenancePriority Priority { get; set; }
    public MaintenanceStatus Status { get; set; }
    public string? AssignedTo { get; set; }
    public DateTime? CompletedAt { get; set; }
    public decimal? EstimatedCost { get; set; }
    public decimal? ActualCost { get; set; }

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
    public User? RequestedByUser { get; set; }
    public ICollection<MaintenanceComment> Comments { get; set; } = new List<MaintenanceComment>();
}
