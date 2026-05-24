namespace ApartmentManagement.Application.Common.DTOs;

public class ResidentDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ApartmentId { get; set; }
    public string? ApartmentNumber { get; set; }
    public Guid? UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string ResidentType { get; set; } = string.Empty;
    public DateTime MoveInDate { get; set; }
    public DateTime? MoveOutDate { get; set; }
    public bool IsPrimaryContact { get; set; }
    public bool HasSystemAccount { get; set; }
}

public class ResidentDetailDto : ResidentDto
{
    public string? BuildingName { get; set; }
    public bool? IsUserActive { get; set; }
}

public class InviteResponseDto
{
    public Guid InviteId { get; set; }
    public string InviteToken { get; set; } = string.Empty;
    public string InviteUrl { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
