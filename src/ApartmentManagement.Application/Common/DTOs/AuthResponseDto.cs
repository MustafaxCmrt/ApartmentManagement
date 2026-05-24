namespace ApartmentManagement.Application.Common.DTOs;

public class AuthResponseDto
{
    public Guid UserId { get; set; }
    public Guid? TenantId { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public int ExpiresIn { get; set; }
    public LoggedInUserDto User { get; set; } = new();
}
