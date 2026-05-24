using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Buildings.Queries.GetBuildingApartments;

public class GetBuildingApartmentsHandler : IRequestHandler<GetBuildingApartmentsQuery, Result<List<ApartmentDto>>>
{
    private readonly IApplicationDbContext _db;

    public GetBuildingApartmentsHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<List<ApartmentDto>>> Handle(GetBuildingApartmentsQuery request, CancellationToken ct)
    {
        var buildingExists = await _db.Buildings.AnyAsync(b => b.Id == request.BuildingId, ct);
        if (!buildingExists)
            return Result<List<ApartmentDto>>.Failure(Error.NotFound("Building"));

        var apartments = await _db.Apartments.AsNoTracking()
            .Where(d => d.BuildingId == request.BuildingId)
            .OrderBy(d => d.Floor).ThenBy(d => d.ApartmentNumber)
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

        return Result<List<ApartmentDto>>.Success(apartments);
    }
}
