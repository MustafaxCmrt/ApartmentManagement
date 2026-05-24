using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using ApartmentManagement.Domain.Entities;
using ApartmentManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Apartments.Commands.CreateApartmentsBatch;

public class CreateApartmentsBatchHandler : IRequestHandler<CreateApartmentsBatchCommand, Result<List<ApartmentDto>>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentTenantService _currentTenant;

    public CreateApartmentsBatchHandler(IApplicationDbContext db, ICurrentTenantService currentTenant)
    {
        _db = db;
        _currentTenant = currentTenant;
    }

    public async Task<Result<List<ApartmentDto>>> Handle(CreateApartmentsBatchCommand request, CancellationToken ct)
    {
        if (_currentTenant.TenantId is not { } tenantId)
            return Result<List<ApartmentDto>>.Failure(Error.Unauthorized("Tenant is required."));

        var building = await _db.Buildings.FirstOrDefaultAsync(b => b.Id == request.BuildingId, ct);
        if (building is null)
            return Result<List<ApartmentDto>>.Failure(Error.NotFound("Building"));

        var existingNumbers = await _db.Apartments
            .Where(d => d.BuildingId == request.BuildingId)
            .Select(d => d.ApartmentNumber)
            .ToListAsync(ct);

        var existingSet = new HashSet<string>(existingNumbers, StringComparer.OrdinalIgnoreCase);
        var batchNumbers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var newApartments = new List<Apartment>();
        var dtos = new List<ApartmentDto>();

        foreach (var item in request.Apartments)
        {
            if (existingSet.Contains(item.ApartmentNumber) || !batchNumbers.Add(item.ApartmentNumber))
                return Result<List<ApartmentDto>>.Failure(Error.Conflict($"Apartment number '{item.ApartmentNumber}' already exists or is duplicated in the batch."));

            if (!Enum.TryParse<OccupancyStatus>(item.OccupancyStatus, true, out var occupancyStatus))
                return Result<List<ApartmentDto>>.Failure(Error.Validation($"Invalid occupancy status: {item.OccupancyStatus}"));

            var apartment = new Apartment
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                BuildingId = request.BuildingId,
                ApartmentNumber = item.ApartmentNumber.Trim(),
                Floor = item.Floor,
                GrossSquareMeters = item.GrossSquareMeters,
                NetSquareMeters = item.NetSquareMeters,
                OccupancyStatus = occupancyStatus,
                DueMultiplier = item.DueMultiplier ?? 1.0m
            };

            newApartments.Add(apartment);
            dtos.Add(new ApartmentDto
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

        _db.Apartments.AddRange(newApartments);
        await _db.SaveChangesAsync(ct);

        return Result<List<ApartmentDto>>.Success(dtos);
    }
}
