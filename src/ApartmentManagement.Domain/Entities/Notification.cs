using ApartmentManagement.Domain.Common;
using ApartmentManagement.Domain.Enums;

namespace ApartmentManagement.Domain.Entities;

public class Notification : BaseEntity, ITenantEntity, ISoftDeletable
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public NotificationType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Link { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime CreatedAt { get; set; }

    // ISoftDeletable
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    // Navigation
    public User? User { get; set; }
}
