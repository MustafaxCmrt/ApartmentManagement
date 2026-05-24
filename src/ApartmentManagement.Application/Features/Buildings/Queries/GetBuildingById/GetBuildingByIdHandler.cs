using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Buildings.Queries.GetBuildingById;

public class GetBuildingByIdHandler : IRequestHandler<GetBuildingByIdQuery, Result<BuildingDetailDto>>
{
    private readonly IApplicationDbContext _db;

    public GetBuildingByIdHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<BuildingDetailDto>> Handle(GetBuildingByIdQuery request, CancellationToken ct)
    {
        var building = await _db.Buildings.AsNoTracking()
            .Where(b => b.Id == request.Id)
            .Select(b => new BuildingDetailDto
            {
                Id = b.Id,
                TenantId = b.TenantId,
                Name = b.Name,
                Address = b.Address,
                FloorCount = b.FloorCount,
                ApartmentCount = b.ApartmentCount,
                ConstructionYear = b.ConstructionYear,
                CreatedAt = b.CreatedAt,
                RegisteredApartmentCount = _db.Apartments.Count(d => d.BuildingId == b.Id),
                ActiveResidentCount = _db.Residents.Count(s => s.Apartment != null && s.Apartment.BuildingId == b.Id && s.MoveOutDate == null)
            })
            .FirstOrDefaultAsync(ct);

        if (building is null)
            return Result<BuildingDetailDto>.Failure(Error.NotFound("Building"));

        return Result<BuildingDetailDto>.Success(building);
    }
}
