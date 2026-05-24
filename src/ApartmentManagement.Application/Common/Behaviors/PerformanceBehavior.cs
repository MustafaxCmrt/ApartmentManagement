using ApartmentManagement.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ApartmentManagement.Application.Common.Behaviors;

public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private const int ThresholdMs = 500;

    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    private readonly ICurrentUserService _currentUser;
    private readonly ICurrentTenantService _currentTenant;

    public PerformanceBehavior(
        ILogger<PerformanceBehavior<TRequest, TResponse>> logger,
        ICurrentUserService currentUser,
        ICurrentTenantService currentTenant)
    {
        _logger = logger;
        _currentUser = currentUser;
        _currentTenant = currentTenant;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();
        var response = await next();
        sw.Stop();

        if (sw.ElapsedMilliseconds > ThresholdMs)
        {
            var requestName = typeof(TRequest).Name;
            _logger.LogWarning(
                "[YAVAŞ] Request: {RequestName} - Elapsed: {Elapsed}ms - UserId: {UserId} - TenantId: {TenantId}",
                requestName, sw.ElapsedMilliseconds, _currentUser.UserId, _currentTenant.TenantId);
        }

        return response;
    }
}
