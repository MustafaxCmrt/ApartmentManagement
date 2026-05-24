using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Utilities;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using ApartmentManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Tenants.Commands.CreateTenant;

public class CreateTenantHandler : IRequestHandler<CreateTenantCommand, Result<TenantDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly IDateTimeProvider _dateTime;

    public CreateTenantHandler(IApplicationDbContext db, IDateTimeProvider dateTime)
    {
        _db = db;
        _dateTime = dateTime;
    }

    public async Task<Result<TenantDto>> Handle(CreateTenantCommand request, CancellationToken ct)
    {
        var shortName = request.ShortName.Trim().ToLowerInvariant();

        var exists = await _db.Tenants.AnyAsync(t => t.ShortName == shortName, ct);
        if (exists)
            return Result<TenantDto>.Failure(Error.Conflict("This short name is already in use."));

        var now = _dateTime.UtcNow;

        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            ShortName = shortName,
            IsActive = true,
            SubscriptionStart = now,
            SubscriptionEnd = request.SubscriptionEnd,
            MaxApartmentCount = request.MaxApartmentCount,
            LogoUrl = request.LogoUrl,
            ContactEmail = EmailNormalizer.Normalize(request.ContactEmail),
            ContactPhone = string.IsNullOrWhiteSpace(request.ContactPhone)
                ? null
                : PhoneNormalizer.Normalize(request.ContactPhone),
            Address = request.Address?.Trim(),
            CreatedAt = now,
            UpdatedAt = now
        };

        _db.Tenants.Add(tenant);
        await _db.SaveChangesAsync(ct);

        return Result<TenantDto>.Success(new TenantDto
        {
            Id = tenant.Id,
            Name = tenant.Name,
            ShortName = tenant.ShortName,
            IsActive = tenant.IsActive,
            SubscriptionStart = tenant.SubscriptionStart,
            SubscriptionEnd = tenant.SubscriptionEnd,
            MaxApartmentCount = tenant.MaxApartmentCount,
            LogoUrl = tenant.LogoUrl,
            ContactEmail = tenant.ContactEmail,
            ContactPhone = tenant.ContactPhone,
            Address = tenant.Address,
            CreatedAt = tenant.CreatedAt,
            UpdatedAt = tenant.UpdatedAt
        });
    }
}
