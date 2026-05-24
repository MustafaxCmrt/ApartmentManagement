using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Buildings.Queries.GetAllBuildings;

public class GetAllBuildingsHandler : IRequestHandler<GetAllBuildingsQuery, Result<PagedResult<BuildingDto>>>
{
    private readonly IApplicationDbContext _db;

    public GetAllBuildingsHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<PagedResult<BuildingDto>>> Handle(GetAllBuildingsQuery request, CancellationToken ct)
    {
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var pageNumber = Math.Max(1, request.PageNumber);

        var query = _db.Buildings.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim();
            query = query.Where(b => b.Name.Contains(s) || b.Address.Contains(s));
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderBy(b => b.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new BuildingDto
            {
                Id = b.Id,
                TenantId = b.TenantId,
                Name = b.Name,
                Address = b.Address,
                FloorCount = b.FloorCount,
                ApartmentCount = b.ApartmentCount,
                ConstructionYear = b.ConstructionYear,
                CreatedAt = b.CreatedAt
            })
            .ToListAsync(ct);

        return Result<PagedResult<BuildingDto>>.Success(new PagedResult<BuildingDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = total
        });
    }
}
