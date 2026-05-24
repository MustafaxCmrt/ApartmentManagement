using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using ApartmentManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Dues.Queries.GetApartmentSummaryReport;

public class GetApartmentSummaryReportHandler : IRequestHandler<GetApartmentSummaryReportQuery, Result<List<ApartmentSummaryDto>>>
{
    private readonly IApplicationDbContext _db;

    public GetApartmentSummaryReportHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<List<ApartmentSummaryDto>>> Handle(GetApartmentSummaryReportQuery request, CancellationToken ct)
    {
        var dq = _db.Apartments.AsNoTracking();
        if (request.BuildingId.HasValue)
            dq = dq.Where(d => d.BuildingId == request.BuildingId.Value);

        var result = await dq
            .OrderBy(d => d.BuildingId).ThenBy(d => d.ApartmentNumber)
            .Select(d => new ApartmentSummaryDto
            {
                ApartmentId = d.Id,
                ApartmentNumber = d.ApartmentNumber,
                BuildingName = d.Building != null ? d.Building.Name : null,
                TotalDueCount = _db.Dues.Count(a => a.ApartmentId == d.Id),
                TotalDebt = _db.Dues.Where(a => a.ApartmentId == d.Id).Sum(a => (decimal?)a.Amount) ?? 0m,
                TotalPaid = _db.DuePayments.Where(o => o.Due != null && o.Due.ApartmentId == d.Id)
                    .Sum(o => (decimal?)o.PaidAmount) ?? 0m,
                OverdueDueCount = _db.Dues.Count(a => a.ApartmentId == d.Id && a.Status == DueStatus.Overdue)
            })
            .ToListAsync(ct);

        foreach (var r in result)
            r.RemainingDebt = r.TotalDebt - r.TotalPaid;

        return Result<List<ApartmentSummaryDto>>.Success(result);
    }
}
