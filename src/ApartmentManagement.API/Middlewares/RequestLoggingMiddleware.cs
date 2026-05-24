using ApartmentManagement.Application.Common.Interfaces;
using Serilog.Context;

namespace ApartmentManagement.API.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, ICurrentUserService currentUser, ICurrentTenantService currentTenant)
    {
        var tenantId = currentTenant.TenantId?.ToString() ?? "none";
        var userId = currentUser.UserId?.ToString() ?? "anonymous";

        using (LogContext.PushProperty("TenantId", tenantId))
        using (LogContext.PushProperty("UserId", userId))
        {
            await _next(context);
        }
    }
}
