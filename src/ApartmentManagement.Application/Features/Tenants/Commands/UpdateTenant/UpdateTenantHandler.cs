using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Tenants.Commands.UpdateTenant;

public class UpdateTenantHandler : IRequestHandler<UpdateTenantCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly IDateTimeProvider _dateTime;

    public UpdateTenantHandler(IApplicationDbContext db, IDateTimeProvider dateTime)
    {
        _db = db;
        _dateTime = dateTime;
    }

    public async Task<Result> Handle(UpdateTenantCommand request, CancellationToken ct)
    {
        var tenant = await _db.Tenants.FirstOrDefaultAsync(t => t.Id == request.Id, ct);
        if (tenant is null)
            return Result.Failure(Error.NotFound("Tenant"));

        tenant.Name = request.Name.Trim();
        tenant.ContactEmail = request.ContactEmail.Trim();
        tenant.ContactPhone = request.ContactPhone?.Trim();
        tenant.Address = request.Address?.Trim();
        tenant.MaxApartmentCount = request.MaxApartmentCount;
        tenant.SubscriptionEnd = request.SubscriptionEnd;
        tenant.LogoUrl = request.LogoUrl;
        tenant.UpdatedAt = _dateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
