using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Tenants.Queries.GetAllTenants;

public class GetAllTenantsHandler : IRequestHandler<GetAllTenantsQuery, Result<PagedResult<TenantDto>>>
{
    private readonly IApplicationDbContext _db;

    public GetAllTenantsHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<PagedResult<TenantDto>>> Handle(GetAllTenantsQuery request, CancellationToken ct)
    {
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var pageNumber = Math.Max(1, request.PageNumber);

        var query = _db.Tenants.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim();
            query = query.Where(t => t.Name.Contains(s) || t.ShortName.Contains(s));
        }

        if (request.IsActive.HasValue)
            query = query.Where(t => t.IsActive == request.IsActive.Value);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TenantDto
            {
                Id = t.Id,
                Name = t.Name,
                ShortName = t.ShortName,
                IsActive = t.IsActive,
                SubscriptionStart = t.SubscriptionStart,
                SubscriptionEnd = t.SubscriptionEnd,
                MaxApartmentCount = t.MaxApartmentCount,
                LogoUrl = t.LogoUrl,
                ContactEmail = t.ContactEmail,
                ContactPhone = t.ContactPhone,
                Address = t.Address,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            })
            .ToListAsync(ct);

        return Result<PagedResult<TenantDto>>.Success(new PagedResult<TenantDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = total
        });
    }
}
