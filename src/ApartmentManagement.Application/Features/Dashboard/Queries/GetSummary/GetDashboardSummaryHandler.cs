using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using ApartmentManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Dashboard.Queries.GetSummary;

public class GetSummaryHandler : IRequestHandler<GetSummaryQuery, Result<DashboardSummaryDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly IDateTimeProvider _dateTime;

    public GetSummaryHandler(IApplicationDbContext db, IDateTimeProvider dateTime)
    {
        _db = db;
        _dateTime = dateTime;
    }

    public async Task<Result<DashboardSummaryDto>> Handle(GetSummaryQuery request, CancellationToken ct)
    {
        var today = _dateTime.UtcNow.Date;
        var monthStart = new DateTime(today.Year, today.Month, 1);
        var monthEnd = monthStart.AddMonths(1);

        var totalApartments = await _db.Apartments.CountAsync(ct);
        var activeResidents = await _db.Residents.CountAsync(r => r.MoveOutDate == null, ct);
        var openMaintenanceTickets = await _db.MaintenanceTickets
            .CountAsync(t => t.Status == MaintenanceStatus.Open || t.Status == MaintenanceStatus.InProgress, ct);
        var overdueDues = await _db.Dues
            .CountAsync(d => d.Status != DueStatus.Paid && d.DueDate < today, ct);
        var monthlyCollectedAmount = await _db.DuePayments
            .Where(p => p.PaymentDate >= monthStart && p.PaymentDate < monthEnd)
            .SumAsync(p => (decimal?)p.PaidAmount, ct) ?? 0m;

        var dto = new DashboardSummaryDto
        {
            TotalApartments = totalApartments,
            ActiveResidentCount = activeResidents,
            OpenMaintenanceTicketCount = openMaintenanceTickets,
            OverdueDueCount = overdueDues,
            MonthlyCollectedAmount = monthlyCollectedAmount
        };

        return Result<DashboardSummaryDto>.Success(dto);
    }
}
