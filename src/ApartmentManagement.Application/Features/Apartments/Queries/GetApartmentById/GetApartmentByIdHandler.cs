using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using ApartmentManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Apartments.Queries.GetApartmentById;

public class GetApartmentByIdHandler : IRequestHandler<GetApartmentByIdQuery, Result<ApartmentDetailDto>>
{
    private readonly IApplicationDbContext _db;

    public GetApartmentByIdHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<ApartmentDetailDto>> Handle(GetApartmentByIdQuery request, CancellationToken ct)
    {
        var dto = await _db.Apartments.AsNoTracking()
            .Where(d => d.Id == request.Id)
            .Select(d => new ApartmentDetailDto
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
                DueMultiplier = d.DueMultiplier,
                ActiveResidentCount = _db.Residents.Count(s => s.ApartmentId == d.Id && s.MoveOutDate == null),
                TotalDueCount = _db.Dues.Count(a => a.ApartmentId == d.Id),
                PendingDueCount = _db.Dues.Count(a => a.ApartmentId == d.Id && a.Status == DueStatus.Pending)
            })
            .FirstOrDefaultAsync(ct);

        if (dto is null)
            return Result<ApartmentDetailDto>.Failure(Error.NotFound("Apartment"));

        return Result<ApartmentDetailDto>.Success(dto);
    }
}
