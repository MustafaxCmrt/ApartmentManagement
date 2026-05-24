using ApartmentManagement.Domain.Common;
using ApartmentManagement.Domain.Enums;

namespace ApartmentManagement.Domain.Entities;

public class Meeting : BaseEntity, ITenantEntity, IAuditableEntity, ISoftDeletable
{
    public Guid TenantId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime MeetingDate { get; set; }
    public string Venue { get; set; } = string.Empty;
    public string Agenda { get; set; } = string.Empty;
    public MeetingStatus Status { get; set; }
    public string? MinutesSummary { get; set; }

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
    public ICollection<MeetingParticipant> Participants { get; set; } = new List<MeetingParticipant>();
    public ICollection<Decision> Decisions { get; set; } = new List<Decision>();
}
