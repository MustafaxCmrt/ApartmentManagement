using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Dashboard.Queries.GetDuesTrend;

public class GetDuesTrendHandler : IRequestHandler<GetDuesTrendQuery, Result<DuesTrendDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly IDateTimeProvider _dateTime;

    public GetDuesTrendHandler(IApplicationDbContext db, IDateTimeProvider dateTime)
    {
        _db = db;
        _dateTime = dateTime;
    }

    public async Task<Result<DuesTrendDto>> Handle(GetDuesTrendQuery request, CancellationToken ct)
    {
        var monthCount = Math.Clamp(request.MonthCount, 1, 24);

        var today = _dateTime.UtcNow.Date;
        var monthStart = new DateTime(today.Year, today.Month, 1);
        var startDate = monthStart.AddMonths(-(monthCount - 1));
        var endDate = monthStart.AddMonths(1);

        var expectedRaw = await _db.Dues
            .Where(d => d.Period >= startDate && d.Period < endDate)
            .GroupBy(d => new { d.Period.Year, d.Period.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Total = g.Sum(d => d.Amount) })
            .ToListAsync(ct);

        var collectedRaw = await _db.DuePayments
            .Where(p => p.PaymentDate >= startDate && p.PaymentDate < endDate)
            .GroupBy(p => new { p.PaymentDate.Year, p.PaymentDate.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Total = g.Sum(p => p.PaidAmount) })
            .ToListAsync(ct);

        var items = new List<DuesTrendItem>();
        for (var i = 0; i < monthCount; i++)
        {
            var month = startDate.AddMonths(i);
            var expected = expectedRaw.FirstOrDefault(x => x.Year == month.Year && x.Month == month.Month)?.Total ?? 0m;
            var collected = collectedRaw.FirstOrDefault(x => x.Year == month.Year && x.Month == month.Month)?.Total ?? 0m;

            items.Add(new DuesTrendItem
            {
                Period = month,
                PeriodLabel = month.ToString("yyyy-MM"),
                Expected = expected,
                Collected = collected
            });
        }

        return Result<DuesTrendDto>.Success(new DuesTrendDto { Items = items });
    }
}
