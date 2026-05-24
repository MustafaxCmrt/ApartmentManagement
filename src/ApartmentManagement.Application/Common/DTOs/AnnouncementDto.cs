namespace ApartmentManagement.Application.Common.DTOs;

public class AnnouncementDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string Audience { get; set; } = string.Empty;
    public Guid? BuildingId { get; set; }
    public string? BuildingName { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public bool IsRead { get; set; }
}

public class AnnouncementDetailDto : AnnouncementDto
{
    public int TotalReads { get; set; }
}

public class ReadStatisticsDto
{
    public Guid AnnouncementId { get; set; }
    public int TotalReads { get; set; }
    public int UniqueUserCount { get; set; }
    public DateTime? FirstReadAt { get; set; }
    public DateTime? LastReadAt { get; set; }
}
