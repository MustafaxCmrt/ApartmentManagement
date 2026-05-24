using ApartmentManagement.Domain.Common;

namespace ApartmentManagement.Domain.Entities;

public class MaintenanceComment : BaseEntity, ITenantEntity, IAuditableEntity
{
    public Guid TenantId { get; set; }
    public Guid MaintenanceTicketId { get; set; }
    public Guid UserId { get; set; }
    public string Comment { get; set; } = string.Empty;

    // IAuditableEntity
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    // Navigation
    public MaintenanceTicket? MaintenanceTicket { get; set; }
    public User? User { get; set; }
}
