using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Buildings.Commands.UpdateBuilding;

public class UpdateBuildingHandler : IRequestHandler<UpdateBuildingCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public UpdateBuildingHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateBuildingCommand request, CancellationToken ct)
    {
        var building = await _db.Buildings.FirstOrDefaultAsync(b => b.Id == request.Id, ct);
        if (building is null)
            return Result.Failure(Error.NotFound("Building"));

        building.Name = request.Name.Trim();
        building.Address = request.Address.Trim();
        building.FloorCount = request.FloorCount;
        building.ApartmentCount = request.ApartmentCount;
        building.ConstructionYear = request.ConstructionYear;

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
