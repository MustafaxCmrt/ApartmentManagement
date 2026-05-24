namespace ApartmentManagement.Application.Common.DTOs;

public class DecisionDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid? MeetingId { get; set; }
    public string? MeetingTitle { get; set; }
    public int DecisionNumber { get; set; }
    public DateTime DecisionDate { get; set; }
    public string DecisionTitle { get; set; } = string.Empty;
    public string DecisionText { get; set; } = string.Empty;
    public int? VotersCount { get; set; }
    public int? ApprovalVotes { get; set; }
    public int? RejectionVotes { get; set; }
    public int? AbstentionVotes { get; set; }
}
