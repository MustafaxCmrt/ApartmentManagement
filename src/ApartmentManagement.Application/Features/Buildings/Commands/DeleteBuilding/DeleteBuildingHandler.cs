using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Buildings.Commands.DeleteBuilding;

public class DeleteBuildingHandler : IRequestHandler<DeleteBuildingCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public DeleteBuildingHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteBuildingCommand request, CancellationToken ct)
    {
        var building = await _db.Buildings.FirstOrDefaultAsync(b => b.Id == request.Id, ct);
        if (building is null)
            return Result.Failure(Error.NotFound("Building"));

        var hasApartments = await _db.Apartments.AnyAsync(d => d.BuildingId == request.Id, ct);
        if (hasApartments)
            return Result.Failure(Error.Conflict("This building has linked apartments. Delete the apartments first."));

        _db.Buildings.Remove(building); // soft delete interceptor will handle
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
