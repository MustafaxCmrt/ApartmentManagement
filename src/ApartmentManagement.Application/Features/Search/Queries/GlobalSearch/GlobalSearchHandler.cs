using ApartmentManagement.Application.Common.DTOs;
using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Search.Queries.GlobalSearch;

public class GlobalSearchHandler : IRequestHandler<GlobalSearchQuery, Result<GlobalSearchResultDto>>
{
    private readonly IApplicationDbContext _db;

    public GlobalSearchHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<GlobalSearchResultDto>> Handle(GlobalSearchQuery request, CancellationToken ct)
    {
        var term = request.Q.Trim().ToLower();
        var effectiveLimit = Math.Min(request.Limit, 20);
        var includeAll = request.Types is null || request.Types.Length == 0;
        var wantResidents = includeAll || request.Types!.Contains("residents", StringComparer.OrdinalIgnoreCase);
        var wantApartments = includeAll || request.Types!.Contains("apartments", StringComparer.OrdinalIgnoreCase);
        var wantDues = includeAll || request.Types!.Contains("dues", StringComparer.OrdinalIgnoreCase);

        var result = new GlobalSearchResultDto();

        if (wantResidents)
        {
            result.Residents = await _db.Residents
                .AsNoTracking()
                .Where(r => r.FullName.ToLower().Contains(term)
                         || r.Phone.ToLower().Contains(term)
                         || (r.Email != null && r.Email.ToLower().Contains(term)))
                .Take(effectiveLimit)
                .Select(r => new ResidentSearchItem
                {
                    Id = r.Id,
                    FullName = r.FullName,
                    Phone = r.Phone,
                    ApartmentNumber = r.Apartment!.ApartmentNumber,
                    BuildingName = r.Apartment.Building!.Name
                })
                .ToListAsync(ct);
        }

        if (wantApartments)
        {
            var aptData = await _db.Apartments
                .AsNoTracking()
                .Where(a => a.ApartmentNumber.ToLower().Contains(term)
                         || a.Building!.Name.ToLower().Contains(term))
                .Take(effectiveLimit)
                .Select(a => new
                {
                    a.Id,
                    a.ApartmentNumber,
                    BuildingName = a.Building!.Name,
                    a.Floor,
                    a.OccupancyStatus
                })
                .ToListAsync(ct);

            result.Apartments = aptData.Select(a => new ApartmentSearchItem
            {
                Id = a.Id,
                ApartmentNumber = a.ApartmentNumber,
                BuildingName = a.BuildingName,
                Floor = a.Floor,
                OccupancyStatus = a.OccupancyStatus.ToString()
            }).ToList();
        }

        if (wantDues)
        {
            var periodMatch = System.Text.RegularExpressions.Regex.Match(term, @"^(\d{4})-(\d{2})$");
            var hasPeriod = periodMatch.Success;
            var yyyy = hasPeriod ? int.Parse(periodMatch.Groups[1].Value) : 0;
            var mm = hasPeriod ? int.Parse(periodMatch.Groups[2].Value) : 0;

            var dueData = await _db.Dues
                .AsNoTracking()
                .Where(d => d.Apartment!.ApartmentNumber.ToLower().Contains(term)
                         || (d.Description != null && d.Description.ToLower().Contains(term))
                         || (hasPeriod && d.Period.Year == yyyy && d.Period.Month == mm))
                .Take(effectiveLimit)
                .Select(d => new
                {
                    d.Id,
                    ApartmentNumber = d.Apartment!.ApartmentNumber,
                    d.Period,
                    d.Amount,
                    d.Status
                })
                .ToListAsync(ct);

            result.Dues = dueData.Select(d => new DueSearchItem
            {
                Id = d.Id,
                ApartmentNumber = d.ApartmentNumber,
                Period = d.Period,
                Amount = d.Amount,
                Status = d.Status.ToString()
            }).ToList();
        }

        return Result<GlobalSearchResultDto>.Success(result);
    }
}
