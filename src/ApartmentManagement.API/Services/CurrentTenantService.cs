using ApartmentManagement.Application.Common.Interfaces;

namespace ApartmentManagement.API.Services;

public class CurrentTenantService : ICurrentTenantService
{
    private readonly IHttpContextAccessor _http;

    public CurrentTenantService(IHttpContextAccessor http) => _http = http;

    public Guid? TenantId
    {
        get
        {
            var tid = _http.HttpContext?.User.FindFirst("tid")?.Value;
            return Guid.TryParse(tid, out var id) ? id : null;
        }
    }

    public bool IsSuperAdmin => _http.HttpContext?.User.FindFirst("superadmin")?.Value == "true";
}
