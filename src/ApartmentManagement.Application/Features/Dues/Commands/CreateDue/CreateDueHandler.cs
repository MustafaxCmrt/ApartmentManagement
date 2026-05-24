using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using ApartmentManagement.Domain.Entities;
using ApartmentManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Dues.Commands.CreateDue;

public class CreateDueHandler : IRequestHandler<CreateDueCommand, Result<DueDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentTenantService _currentTenant;

    public CreateDueHandler(IApplicationDbContext db, ICurrentTenantService currentTenant)
    {
        _db = db;
        _currentTenant = currentTenant;
    }

    public async Task<Result<DueDto>> Handle(CreateDueCommand request, CancellationToken ct)
    {
        if (_currentTenant.TenantId is not { } tenantId)
            return Result<DueDto>.Failure(Error.Unauthorized("Tenant is required."));

        var apartment = await _db.Apartments.FirstOrDefaultAsync(d => d.Id == request.ApartmentId, ct);
        if (apartment is null)
            return Result<DueDto>.Failure(Error.NotFound("Apartment"));

        var period = new DateTime(request.Period.Year, request.Period.Month, 1);

        var exists = await _db.Dues
            .AnyAsync(a => a.ApartmentId == request.ApartmentId && a.Period.Year == period.Year && a.Period.Month == period.Month, ct);

        if (exists)
            return Result<DueDto>.Failure(Error.Conflict("A due for this apartment in the same period already exists."));

        var due = new Due
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ApartmentId = request.ApartmentId,
            Period = period,
            Amount = request.Amount,
            DueDate = request.DueDate,
            Description = request.Description,
            Status = DueStatus.Pending,
            CreationType = DueCreationType.Manual
        };

        _db.Dues.Add(due);
        await _db.SaveChangesAsync(ct);

        return Result<DueDto>.Success(new DueDto
        {
            Id = due.Id,
            TenantId = due.TenantId,
            ApartmentId = due.ApartmentId,
            ApartmentNumber = apartment.ApartmentNumber,
            Period = due.Period,
            Amount = due.Amount,
            DueDate = due.DueDate,
            Description = due.Description,
            Status = due.Status.ToString(),
            CreationType = due.CreationType.ToString(),
            TotalPaid = 0,
            RemainingAmount = due.Amount
        });
    }
}
