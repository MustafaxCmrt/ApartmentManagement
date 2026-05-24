namespace ApartmentManagement.Application.Common.DTOs;

public class MeetingDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime MeetingDate { get; set; }
    public string Venue { get; set; } = string.Empty;
    public string Agenda { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? MinutesSummary { get; set; }
    public int ParticipantCount { get; set; }
}

public class MeetingDetailDto : MeetingDto
{
    public List<ParticipantDto> Participants { get; set; } = new();
}

public class ParticipantDto
{
    public Guid Id { get; set; }
    public Guid MeetingId { get; set; }
    public Guid ApartmentId { get; set; }
    public string? ApartmentNumber { get; set; }
    public string AttendanceStatus { get; set; } = string.Empty;
    public Guid? ProxyApartmentId { get; set; }
    public string? ProxyApartmentNumber { get; set; }
}
