using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Residents.Commands.DeleteResidentsBatch;

public class DeleteResidentsBatchHandler : IRequestHandler<DeleteResidentsBatchCommand, Result<DeleteResidentsBatchResult>>
{
    private readonly IApplicationDbContext _db;

    public DeleteResidentsBatchHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<DeleteResidentsBatchResult>> Handle(DeleteResidentsBatchCommand request, CancellationToken ct)
    {
        var uniqueIds = request.Ids.ToHashSet();

        var residents = await _db.Residents
            .Where(r => uniqueIds.Contains(r.Id))
            .ToListAsync(ct);

        var foundIds = residents.Select(r => r.Id).ToHashSet();
        var notFound = uniqueIds.Where(id => !foundIds.Contains(id)).ToList();

        var affectedApartmentIds = residents.Select(r => r.ApartmentId).Distinct().ToList();

        foreach (var resident in residents)
            _db.Residents.Remove(resident);

        if (residents.Count > 0)
            await _db.SaveChangesAsync(ct);

        if (affectedApartmentIds.Count > 0)
        {
            var activeResidentTypes = await _db.Residents
                .Where(r => affectedApartmentIds.Contains(r.ApartmentId) && r.MoveOutDate == null)
                .Select(r => new { r.ApartmentId, r.ResidentType })
                .ToListAsync(ct);

            var typesByApartment = activeResidentTypes
                .GroupBy(r => r.ApartmentId)
                .ToDictionary(g => g.Key, g => g.Select(r => r.ResidentType).ToList());

            var apartments = await _db.Apartments
                .Where(a => affectedApartmentIds.Contains(a.Id))
                .ToListAsync(ct);

            foreach (var apartment in apartments)
            {
                var types = typesByApartment.TryGetValue(apartment.Id, out var t) ? t : new List<ResidentType>();

                apartment.OccupancyStatus = types.Any(rt => rt == ResidentType.Owner)
                    ? OccupancyStatus.Occupied
                    : types.Any(rt => rt == ResidentType.Tenant)
                        ? OccupancyStatus.Rented
                        : OccupancyStatus.Vacant;
            }

            await _db.SaveChangesAsync(ct);
        }

        var deleted = residents.Select(r => r.Id).ToList();

        return Result<DeleteResidentsBatchResult>.Success(
            new DeleteResidentsBatchResult(deleted, notFound));
    }
}
