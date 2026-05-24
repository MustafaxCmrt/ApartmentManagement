using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Tenants.Commands.DeleteTenant;

public class DeleteTenantHandler : IRequestHandler<DeleteTenantCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public DeleteTenantHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteTenantCommand request, CancellationToken ct)
    {
        var tenant = await _db.Tenants.FirstOrDefaultAsync(t => t.Id == request.Id, ct);
        if (tenant is null)
            return Result.Failure(Error.NotFound("Tenant"));

        // Hard delete (cascade — DB configuration handles cascading)
        _db.Tenants.Remove(tenant);
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
