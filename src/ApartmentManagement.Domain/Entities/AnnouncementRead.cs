using ApartmentManagement.Domain.Common;

namespace ApartmentManagement.Domain.Entities;

public class AnnouncementRead : BaseEntity, ITenantEntity
{
    public Guid TenantId { get; set; }
    public Guid AnnouncementId { get; set; }
    public Guid UserId { get; set; }
    public DateTime ReadAt { get; set; }

    // Navigation
    public Announcement? Announcement { get; set; }
    public User? User { get; set; }
}
