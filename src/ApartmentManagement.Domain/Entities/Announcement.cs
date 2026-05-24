using ApartmentManagement.Domain.Common;
using ApartmentManagement.Domain.Enums;

namespace ApartmentManagement.Domain.Entities;

public class Announcement : BaseEntity, ITenantEntity, IAuditableEntity, ISoftDeletable
{
    public Guid TenantId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public AnnouncementSeverity Severity { get; set; }
    public DateTime PublishedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public AnnouncementAudience Audience { get; set; }
    public Guid? BuildingId { get; set; }

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
    public ICollection<AnnouncementRead> Reads { get; set; } = new List<AnnouncementRead>();
}
