using ApartmentManagement.Application.Common.Interfaces;
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

        resident.FullName = request.FullName.Trim();
        resident.Phone = request.Phone.Trim();
        resident.Email = request.Email?.Trim();
        resident.ResidentType = residentType;
        resident.MoveInDate = request.MoveInDate;
        resident.MoveOutDate = request.MoveOutDate;
        resident.IsPrimaryContact = request.IsPrimaryContact;

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
