namespace ApartmentManagement.Application.Common.DTOs;

public class MaintenanceTicketDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid? ApartmentId { get; set; }
    public string? ApartmentNumber { get; set; }
    public Guid RequestedByUserId { get; set; }
    public string? RequestedByFullName { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? AssignedTo { get; set; }
    public DateTime? CompletedAt { get; set; }
    public decimal? EstimatedCost { get; set; }
    public decimal? ActualCost { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class MaintenanceTicketDetailDto : MaintenanceTicketDto
{
    public int CommentCount { get; set; }
}

public class MaintenanceCommentDto
{
    public Guid Id { get; set; }
    public Guid MaintenanceTicketId { get; set; }
    public Guid UserId { get; set; }
    public string? UserFullName { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
