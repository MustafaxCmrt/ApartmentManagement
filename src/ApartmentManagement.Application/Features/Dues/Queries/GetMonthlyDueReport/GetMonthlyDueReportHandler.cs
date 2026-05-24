using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using ApartmentManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Dues.Queries.GetMonthlyDueReport;

public class GetMonthlyDueReportHandler : IRequestHandler<GetMonthlyDueReportQuery, Result<MonthlyReportDto>>
{
    private readonly IApplicationDbContext _db;

    public GetMonthlyDueReportHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<MonthlyReportDto>> Handle(GetMonthlyDueReportQuery request, CancellationToken ct)
    {
        if (request.Year < 2000 || request.Year > 2100 || request.Month < 1 || request.Month > 12)
            return Result<MonthlyReportDto>.Failure(Error.Validation("Invalid year/month."));

        var period = new DateTime(request.Year, request.Month, 1);

        var dues = await _db.Dues.AsNoTracking()
            .Where(a => a.Period.Year == period.Year && a.Period.Month == period.Month)
            .Select(a => new
            {
                a.Id,
                a.Amount,
                a.Status,
                Paid = a.Payments.Sum(o => (decimal?)o.PaidAmount) ?? 0m
            })
            .ToListAsync(ct);

        var dto = new MonthlyReportDto
        {
            Period = period,
            TotalDueCount = dues.Count,
            TotalAmount = dues.Sum(a => a.Amount),
            CollectedAmount = dues.Sum(a => a.Paid),
            RemainingAmount = dues.Sum(a => a.Amount - a.Paid),
            PaidDueCount = dues.Count(a => a.Status == DueStatus.Paid),
            PendingDueCount = dues.Count(a => a.Status == DueStatus.Pending),
            OverdueDueCount = dues.Count(a => a.Status == DueStatus.Overdue),
            PartiallyPaidCount = dues.Count(a => a.Status == DueStatus.PartiallyPaid)
        };

        return Result<MonthlyReportDto>.Success(dto);
    }
}
