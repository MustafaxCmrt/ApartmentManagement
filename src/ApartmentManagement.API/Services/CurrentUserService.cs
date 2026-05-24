using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ApartmentManagement.Application.Common.Interfaces;

namespace ApartmentManagement.API.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _http;

    public CurrentUserService(IHttpContextAccessor http) => _http = http;

    public Guid? UserId
    {
        get
        {
            var sub = _http.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                   ?? _http.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(sub, out var id) ? id : null;
        }
    }

    public string? Email => _http.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Email)?.Value
                         ?? _http.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;

    public string? Role => _http.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;

    public bool IsAuthenticated => _http.HttpContext?.User.Identity?.IsAuthenticated == true;

    public string? IpAddress => _http.HttpContext?.Connection.RemoteIpAddress?.ToString();
}
