using ApartmentManagement.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ApartmentManagement.Application.Common.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    private readonly ICurrentUserService _currentUser;
    private readonly ICurrentTenantService _currentTenant;

    public LoggingBehavior(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger,
        ICurrentUserService currentUser,
        ICurrentTenantService currentTenant)
    {
        _logger = logger;
        _currentUser = currentUser;
        _currentTenant = currentTenant;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _currentUser.UserId;
        var tenantId = _currentTenant.TenantId;

        _logger.LogInformation(
            "[BAŞLADI] Request: {RequestName} - UserId: {UserId} - TenantId: {TenantId}",
            requestName, userId, tenantId);

        var sw = Stopwatch.StartNew();
        try
        {
            var response = await next();
            sw.Stop();

            _logger.LogInformation(
                "[BİTTİ] Request: {RequestName} - UserId: {UserId} - TenantId: {TenantId} - Elapsed: {Elapsed}ms",
                requestName, userId, tenantId, sw.ElapsedMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex,
                "[HATA] Request: {RequestName} - UserId: {UserId} - TenantId: {TenantId} - Elapsed: {Elapsed}ms",
                requestName, userId, tenantId, sw.ElapsedMilliseconds);
            throw;
        }
    }
}
