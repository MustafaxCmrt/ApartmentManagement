using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Buildings.Commands.DeleteBuildingsBatch;

public class DeleteBuildingsBatchHandler : IRequestHandler<DeleteBuildingsBatchCommand, Result<DeleteBuildingsBatchResult>>
{
    private readonly IApplicationDbContext _db;

    public DeleteBuildingsBatchHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<DeleteBuildingsBatchResult>> Handle(DeleteBuildingsBatchCommand request, CancellationToken ct)
    {
        var uniqueIds = request.Ids.ToHashSet();

        var buildings = await _db.Buildings
            .Where(b => uniqueIds.Contains(b.Id))
            .ToListAsync(ct);

        var foundIds = buildings.Select(b => b.Id).ToHashSet();
        var notFound = uniqueIds.Where(id => !foundIds.Contains(id)).ToList();

        var buildingIds = buildings.Select(b => b.Id).ToList();
        var buildingsWithApartments = await _db.Apartments
            .Where(a => buildingIds.Contains(a.BuildingId))
            .Select(a => a.BuildingId)
            .Distinct()
            .ToListAsync(ct);

        var blockedIds = buildingsWithApartments.ToHashSet();

        var toDelete = buildings.Where(b => !blockedIds.Contains(b.Id)).ToList();
        var skipped = buildings.Where(b => blockedIds.Contains(b.Id)).Select(b => b.Id).ToList();

        foreach (var building in toDelete)
            _db.Buildings.Remove(building);

        if (toDelete.Count > 0)
            await _db.SaveChangesAsync(ct);

        var deleted = toDelete.Select(b => b.Id).ToList();

        return Result<DeleteBuildingsBatchResult>.Success(
            new DeleteBuildingsBatchResult(deleted, notFound, skipped));
    }
}
