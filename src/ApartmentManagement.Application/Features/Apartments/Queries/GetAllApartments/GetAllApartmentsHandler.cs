using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using ApartmentManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Apartments.Queries.GetAllApartments;

public class GetAllApartmentsHandler : IRequestHandler<GetAllApartmentsQuery, Result<PagedResult<ApartmentDto>>>
{
    private readonly IApplicationDbContext _db;

    public GetAllApartmentsHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<PagedResult<ApartmentDto>>> Handle(GetAllApartmentsQuery request, CancellationToken ct)
    {
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var pageNumber = Math.Max(1, request.PageNumber);

        var query = _db.Apartments.AsNoTracking();

        if (request.BuildingId.HasValue)
            query = query.Where(d => d.BuildingId == request.BuildingId.Value);

        if (!string.IsNullOrWhiteSpace(request.OccupancyStatus) &&
            Enum.TryParse<OccupancyStatus>(request.OccupancyStatus, true, out var os))
            query = query.Where(d => d.OccupancyStatus == os);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderBy(d => d.BuildingId).ThenBy(d => d.Floor).ThenBy(d => d.ApartmentNumber)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(d => new ApartmentDto
            {
                Id = d.Id,
                TenantId = d.TenantId,
                BuildingId = d.BuildingId,
                BuildingName = d.Building != null ? d.Building.Name : null,
                ApartmentNumber = d.ApartmentNumber,
                Floor = d.Floor,
                GrossSquareMeters = d.GrossSquareMeters,
                NetSquareMeters = d.NetSquareMeters,
                OccupancyStatus = d.OccupancyStatus.ToString(),
                DueMultiplier = d.DueMultiplier
            })
            .ToListAsync(ct);

        return Result<PagedResult<ApartmentDto>>.Success(new PagedResult<ApartmentDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = total
        });
    }
}
