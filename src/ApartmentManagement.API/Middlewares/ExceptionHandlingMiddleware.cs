using System.Net;
using System.Text.Json;
using ApartmentManagement.Application.Common.Exceptions;
using ApartmentManagement.Domain.Exceptions;

namespace ApartmentManagement.API.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleAsync(context, ex);
        }
    }

    private async Task HandleAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        object payload;

        switch (exception)
        {
            case ValidationException validation:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                payload = new
                {
                    code = "Validation",
                    message = validation.Message,
                    errors = validation.Errors
                };
                _logger.LogWarning(validation, "Validation hatası: {Message}", validation.Message);
                break;

            case TenantMismatchException tenantMismatch:
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                payload = new { code = "Forbidden", message = tenantMismatch.Message };
                _logger.LogWarning(tenantMismatch, "Tenant uyuşmazlığı");
                break;

            case DomainException domain:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                payload = new { code = "Validation", message = domain.Message };
                _logger.LogWarning(domain, "Domain hatası: {Message}", domain.Message);
                break;

            case UnauthorizedAccessException:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                payload = new { code = "Unauthorized", message = "Yetkisiz erişim." };
                _logger.LogWarning(exception, "Yetkisiz erişim");
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var message = _env.IsDevelopment() ? exception.Message : "Bir hata oluştu.";
                payload = new { code = "Failure", message };
                _logger.LogError(exception, "Beklenmeyen hata: {Message}", exception.Message);
                break;
        }

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await context.Response.WriteAsync(JsonSerializer.Serialize(payload, options));
    }
}
