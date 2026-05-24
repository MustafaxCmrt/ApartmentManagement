using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Tenants.Commands.ToggleActiveStatus;

public class ToggleActiveStatusHandler : IRequestHandler<ToggleActiveStatusCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly IDateTimeProvider _dateTime;

    public ToggleActiveStatusHandler(IApplicationDbContext db, IDateTimeProvider dateTime)
    {
        _db = db;
        _dateTime = dateTime;
    }

    public async Task<Result> Handle(ToggleActiveStatusCommand request, CancellationToken ct)
    {
        var tenant = await _db.Tenants.FirstOrDefaultAsync(t => t.Id == request.Id, ct);
        if (tenant is null)
            return Result.Failure(Error.NotFound("Tenant"));

        tenant.IsActive = request.IsActive;
        tenant.UpdatedAt = _dateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
