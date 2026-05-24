using ApartmentManagement.Domain.Common;

namespace ApartmentManagement.Domain.Entities;

public class Decision : BaseEntity, ITenantEntity, IAuditableEntity, ISoftDeletable
{
    public Guid TenantId { get; set; }
    public Guid? MeetingId { get; set; }
    public int DecisionNumber { get; set; }
    public DateTime DecisionDate { get; set; }
    public string DecisionTitle { get; set; } = string.Empty;
    public string DecisionText { get; set; } = string.Empty;
    public int? VotersCount { get; set; }
    public int? ApprovalVotes { get; set; }
    public int? RejectionVotes { get; set; }
    public int? AbstentionVotes { get; set; }

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
    public Meeting? Meeting { get; set; }
}
