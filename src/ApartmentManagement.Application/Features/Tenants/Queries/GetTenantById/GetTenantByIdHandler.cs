using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Tenants.Queries.GetTenantById;

public class GetTenantByIdHandler : IRequestHandler<GetTenantByIdQuery, Result<TenantDto>>
{
    private readonly IApplicationDbContext _db;

    public GetTenantByIdHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<TenantDto>> Handle(GetTenantByIdQuery request, CancellationToken ct)
    {
        var dto = await _db.Tenants
            .AsNoTracking()
            .Where(t => t.Id == request.Id)
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
            .FirstOrDefaultAsync(ct);

        if (dto is null)
            return Result<TenantDto>.Failure(Error.NotFound("Tenant"));

        return Result<TenantDto>.Success(dto);
    }
}
