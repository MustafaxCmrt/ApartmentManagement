using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Dues.Queries.GetApartmentDueHistory;

public class GetApartmentDueHistoryHandler : IRequestHandler<GetApartmentDueHistoryQuery, Result<List<DueDto>>>
{
    private readonly IApplicationDbContext _db;

    public GetApartmentDueHistoryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<List<DueDto>>> Handle(GetApartmentDueHistoryQuery request, CancellationToken ct)
    {
        var apartmentExists = await _db.Apartments.AnyAsync(d => d.Id == request.ApartmentId, ct);
        if (!apartmentExists)
            return Result<List<DueDto>>.Failure(Error.NotFound("Apartment"));

        var list = await _db.Dues.AsNoTracking()
            .Where(a => a.ApartmentId == request.ApartmentId)
            .OrderByDescending(a => a.Period)
            .Select(a => new DueDto
            {
                Id = a.Id,
                TenantId = a.TenantId,
                ApartmentId = a.ApartmentId,
                ApartmentNumber = a.Apartment != null ? a.Apartment.ApartmentNumber : null,
                BuildingName = a.Apartment != null && a.Apartment.Building != null ? a.Apartment.Building.Name : null,
                Period = a.Period,
                Amount = a.Amount,
                DueDate = a.DueDate,
                Description = a.Description,
                Status = a.Status.ToString(),
                CreationType = a.CreationType.ToString(),
                TotalPaid = a.Payments.Sum(o => (decimal?)o.PaidAmount) ?? 0m,
                RemainingAmount = a.Amount - (a.Payments.Sum(o => (decimal?)o.PaidAmount) ?? 0m)
            })
            .ToListAsync(ct);

        return Result<List<DueDto>>.Success(list);
    }
}
