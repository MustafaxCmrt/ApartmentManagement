using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Apartments.Commands.UpdateApartment;

public class UpdateApartmentHandler : IRequestHandler<UpdateApartmentCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public UpdateApartmentHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateApartmentCommand request, CancellationToken ct)
    {
        var apartment = await _db.Apartments.FirstOrDefaultAsync(d => d.Id == request.Id, ct);
        if (apartment is null)
            return Result.Failure(Error.NotFound("Apartment"));

        if (!Enum.TryParse<OccupancyStatus>(request.OccupancyStatus, true, out var occupancyStatus))
            return Result.Failure(Error.Validation("Invalid occupancy status."));

        // Unique apartment number per building (excluding itself)
        var conflict = await _db.Apartments.AnyAsync(d =>
            d.BuildingId == apartment.BuildingId && d.Id != apartment.Id && d.ApartmentNumber == request.ApartmentNumber, ct);
        if (conflict)
            return Result.Failure(Error.Conflict("An apartment with the same number already exists in this building."));

        apartment.ApartmentNumber = request.ApartmentNumber.Trim();
        apartment.Floor = request.Floor;
        apartment.GrossSquareMeters = request.GrossSquareMeters;
        apartment.NetSquareMeters = request.NetSquareMeters;
        apartment.OccupancyStatus = occupancyStatus;
        apartment.DueMultiplier = request.DueMultiplier;

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
