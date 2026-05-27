using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Residents.Commands.DeleteResident;

public class DeleteResidentHandler : IRequestHandler<DeleteResidentCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public DeleteResidentHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteResidentCommand request, CancellationToken ct)
    {
        var resident = await _db.Residents.FirstOrDefaultAsync(s => s.Id == request.Id, ct);
        if (resident is null)
            return Result.Failure(Error.NotFound("Resident"));

        var apartmentId = resident.ApartmentId;
        _db.Residents.Remove(resident);

        var remainingActiveTypes = await _db.Residents
            .Where(s => s.ApartmentId == apartmentId && s.Id != resident.Id && s.MoveOutDate == null)
            .Select(s => s.ResidentType)
            .ToListAsync(ct);

        var apartment = await _db.Apartments.FirstOrDefaultAsync(d => d.Id == apartmentId, ct);
        if (apartment is not null)
        {
            apartment.OccupancyStatus = remainingActiveTypes.Any(t => t == ResidentType.Owner)
                ? OccupancyStatus.Occupied
                : remainingActiveTypes.Any(t => t == ResidentType.Tenant)
                    ? OccupancyStatus.Rented
                    : OccupancyStatus.Vacant;
        }

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
