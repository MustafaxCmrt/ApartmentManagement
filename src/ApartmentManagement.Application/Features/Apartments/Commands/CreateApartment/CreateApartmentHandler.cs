using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using ApartmentManagement.Domain.Entities;
using ApartmentManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Apartments.Commands.CreateApartment;

public class CreateApartmentHandler : IRequestHandler<CreateApartmentCommand, Result<ApartmentDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentTenantService _currentTenant;

    public CreateApartmentHandler(IApplicationDbContext db, ICurrentTenantService currentTenant)
    {
        _db = db;
        _currentTenant = currentTenant;
    }

    public async Task<Result<ApartmentDto>> Handle(CreateApartmentCommand request, CancellationToken ct)
    {
        if (_currentTenant.TenantId is not { } tenantId)
            return Result<ApartmentDto>.Failure(Error.Unauthorized("Tenant is required."));

        var building = await _db.Buildings.FirstOrDefaultAsync(b => b.Id == request.BuildingId, ct);
        if (building is null)
            return Result<ApartmentDto>.Failure(Error.NotFound("Building"));

        if (!Enum.TryParse<OccupancyStatus>(request.OccupancyStatus, true, out var occupancyStatus))
            return Result<ApartmentDto>.Failure(Error.Validation("Invalid occupancy status."));

        var existsAtFloor = await _db.Apartments
            .AnyAsync(d => d.BuildingId == request.BuildingId && d.ApartmentNumber == request.ApartmentNumber, ct);

        if (existsAtFloor)
            return Result<ApartmentDto>.Failure(Error.Conflict("An apartment with the same number already exists in this building."));

        var apartment = new Apartment
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            BuildingId = request.BuildingId,
            ApartmentNumber = request.ApartmentNumber.Trim(),
            Floor = request.Floor,
            GrossSquareMeters = request.GrossSquareMeters,
            NetSquareMeters = request.NetSquareMeters,
            OccupancyStatus = occupancyStatus,
            DueMultiplier = request.DueMultiplier ?? 1.0m
        };

        _db.Apartments.Add(apartment);
        await _db.SaveChangesAsync(ct);

        return Result<ApartmentDto>.Success(new ApartmentDto
        {
            Id = apartment.Id,
            TenantId = apartment.TenantId,
            BuildingId = apartment.BuildingId,
            BuildingName = building.Name,
            ApartmentNumber = apartment.ApartmentNumber,
            Floor = apartment.Floor,
            GrossSquareMeters = apartment.GrossSquareMeters,
            NetSquareMeters = apartment.NetSquareMeters,
            OccupancyStatus = apartment.OccupancyStatus.ToString(),
            DueMultiplier = apartment.DueMultiplier
        });
    }
}
