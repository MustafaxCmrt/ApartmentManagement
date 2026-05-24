using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Domain.Entities;
using ApartmentManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Dues.Commands.CreateBulkDues;

public class CreateBulkDuesHandler : IRequestHandler<CreateBulkDuesCommand, Result<int>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentTenantService _currentTenant;

    public CreateBulkDuesHandler(IApplicationDbContext db, ICurrentTenantService currentTenant)
    {
        _db = db;
        _currentTenant = currentTenant;
    }

    public async Task<Result<int>> Handle(CreateBulkDuesCommand request, CancellationToken ct)
    {
        if (_currentTenant.TenantId is not { } tenantId)
            return Result<int>.Failure(Error.Unauthorized("Tenant is required."));

        var period = new DateTime(request.Period.Year, request.Period.Month, 1);

        var apartmentsQuery = _db.Apartments.AsNoTracking().Where(d => d.TenantId == tenantId);
        if (request.BuildingId.HasValue)
            apartmentsQuery = apartmentsQuery.Where(d => d.BuildingId == request.BuildingId.Value);

        var apartments = await apartmentsQuery
            .Select(d => new { d.Id, d.DueMultiplier })
            .ToListAsync(ct);

        if (apartments.Count == 0)
            return Result<int>.Failure(Error.NotFound("Apartment"));

        // Existing dues (same period)
        var apartmentIds = apartments.Select(d => d.Id).ToList();
        var existing = await _db.Dues
            .Where(a => apartmentIds.Contains(a.ApartmentId) && a.Period.Year == period.Year && a.Period.Month == period.Month)
            .Select(a => a.ApartmentId)
            .ToListAsync(ct);

        var existingSet = new HashSet<Guid>(existing);
        var newDues = new List<Due>();

        foreach (var d in apartments)
        {
            if (existingSet.Contains(d.Id)) continue;

            newDues.Add(new Due
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ApartmentId = d.Id,
                Period = period,
                Amount = Math.Round(request.BaseAmount * d.DueMultiplier, 2),
                DueDate = request.DueDate,
                Description = request.Description,
                Status = DueStatus.Pending,
                CreationType = DueCreationType.Bulk
            });
        }

        if (newDues.Count == 0)
            return Result<int>.Success(0);

        _db.Dues.AddRange(newDues);
        await _db.SaveChangesAsync(ct);

        return Result<int>.Success(newDues.Count);
    }
}
