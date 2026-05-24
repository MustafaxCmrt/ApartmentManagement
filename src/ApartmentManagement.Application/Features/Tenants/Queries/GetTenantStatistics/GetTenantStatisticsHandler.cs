using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using ApartmentManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Tenants.Queries.GetTenantStatistics;

public class GetTenantStatisticsHandler : IRequestHandler<GetTenantStatisticsQuery, Result<TenantStatisticsDto>>
{
    private readonly IApplicationDbContext _db;

    public GetTenantStatisticsHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<TenantStatisticsDto>> Handle(GetTenantStatisticsQuery request, CancellationToken ct)
    {
        var tenant = await _db.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == request.TenantId, ct);

        if (tenant is null)
            return Result<TenantStatisticsDto>.Failure(Error.NotFound("Tenant"));

        var buildingCount = await _db.Buildings.IgnoreQueryFilters()
            .CountAsync(b => b.TenantId == request.TenantId && !b.IsDeleted, ct);

        var apartmentCount = await _db.Apartments.IgnoreQueryFilters()
            .CountAsync(d => d.TenantId == request.TenantId && !d.IsDeleted, ct);

        var residentCount = await _db.Residents.IgnoreQueryFilters()
            .CountAsync(s => s.TenantId == request.TenantId && !s.IsDeleted, ct);

        var activeUserCount = await _db.Users.IgnoreQueryFilters()
            .CountAsync(u => u.TenantId == request.TenantId && !u.IsDeleted && u.IsActive, ct);

        var pendingDueCount = await _db.Dues.IgnoreQueryFilters()
            .CountAsync(d => d.TenantId == request.TenantId && !d.IsDeleted && d.Status == DueStatus.Pending, ct);

        var overdueDueCount = await _db.Dues.IgnoreQueryFilters()
            .CountAsync(d => d.TenantId == request.TenantId && !d.IsDeleted && d.Status == DueStatus.Overdue, ct);

        var totalCollectedAmount = await _db.DuePayments.IgnoreQueryFilters()
            .Where(p => p.TenantId == request.TenantId)
            .SumAsync(p => (decimal?)p.PaidAmount, ct) ?? 0m;

        var dto = new TenantStatisticsDto
        {
            TenantId = tenant.Id,
            TenantName = tenant.Name,
            TotalBuildingCount = buildingCount,
            TotalApartmentCount = apartmentCount,
            TotalResidentCount = residentCount,
            ActiveUserCount = activeUserCount,
            PendingDueCount = pendingDueCount,
            OverdueDueCount = overdueDueCount,
            TotalCollectedAmount = totalCollectedAmount
        };

        return Result<TenantStatisticsDto>.Success(dto);
    }
}
