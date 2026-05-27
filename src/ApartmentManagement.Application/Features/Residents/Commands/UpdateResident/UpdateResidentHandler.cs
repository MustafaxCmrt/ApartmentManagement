using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Utilities;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Residents.Commands.UpdateResident;

public class UpdateResidentHandler : IRequestHandler<UpdateResidentCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public UpdateResidentHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateResidentCommand request, CancellationToken ct)
    {
        var resident = await _db.Residents.FirstOrDefaultAsync(s => s.Id == request.Id, ct);
        if (resident is null)
            return Result.Failure(Error.NotFound("Resident"));

        if (!Enum.TryParse<ResidentType>(request.ResidentType, true, out var residentType))
            return Result.Failure(Error.Validation("Invalid resident type."));

        if (request.IsPrimaryContact && !resident.IsPrimaryContact)
        {
            var others = await _db.Residents
                .Where(s => s.ApartmentId == resident.ApartmentId && s.Id != resident.Id && s.IsPrimaryContact)
                .ToListAsync(ct);
            foreach (var o in others) o.IsPrimaryContact = false;
        }

        var phone = PhoneNormalizer.Normalize(request.Phone);
        var email = string.IsNullOrWhiteSpace(request.Email) ? null : EmailNormalizer.Normalize(request.Email);

        if (resident.UserId is not null)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == resident.UserId.Value, ct);
            if (user is not null && user.Phone != phone)
            {
                var phoneExists = await _db.Users
                    .AnyAsync(u => !u.IsDeleted && u.Id != user.Id && u.Phone == phone, ct);

                if (phoneExists)
                    return Result.Failure(Error.Conflict("Bu telefon numarası zaten kayıtlı."));

                user.Phone = phone;
            }
        }

        resident.FullName = request.FullName.Trim();
        resident.Phone = phone;
        resident.Email = email;
        resident.ResidentType = residentType;
        resident.MoveInDate = request.MoveInDate;
        resident.MoveOutDate = request.MoveOutDate;
        resident.IsPrimaryContact = request.IsPrimaryContact;

        var activeTypes = await _db.Residents
            .Where(s => s.ApartmentId == resident.ApartmentId && s.MoveOutDate == null)
            .Select(s => new { s.Id, s.ResidentType })
            .ToListAsync(ct);

        var updatedActiveTypes = activeTypes
            .Where(s => s.Id != resident.Id)
            .Select(s => s.ResidentType)
            .Concat(request.MoveOutDate == null ? new[] { residentType } : Array.Empty<ResidentType>())
            .ToList();

        var apartment = await _db.Apartments.FirstOrDefaultAsync(d => d.Id == resident.ApartmentId, ct);
        if (apartment is not null)
        {
            apartment.OccupancyStatus = updatedActiveTypes.Any(t => t == ResidentType.Owner)
                ? OccupancyStatus.Occupied
                : updatedActiveTypes.Any(t => t == ResidentType.Tenant)
                    ? OccupancyStatus.Rented
                    : OccupancyStatus.Vacant;
        }

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
