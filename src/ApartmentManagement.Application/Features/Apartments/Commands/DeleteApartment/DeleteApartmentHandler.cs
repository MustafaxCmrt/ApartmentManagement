using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Apartments.Commands.DeleteApartment;

public class DeleteApartmentHandler : IRequestHandler<DeleteApartmentCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public DeleteApartmentHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteApartmentCommand request, CancellationToken ct)
    {
        var apartment = await _db.Apartments.FirstOrDefaultAsync(d => d.Id == request.Id, ct);
        if (apartment is null)
            return Result.Failure(Error.NotFound("Apartment"));

        var hasResidents = await _db.Residents.AnyAsync(s => s.ApartmentId == request.Id, ct);
        if (hasResidents)
            return Result.Failure(Error.Conflict("This apartment has linked residents."));

        var hasDues = await _db.Dues.AnyAsync(a => a.ApartmentId == request.Id, ct);
        if (hasDues)
            return Result.Failure(Error.Conflict("This apartment has linked due records."));

        _db.Apartments.Remove(apartment);
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
