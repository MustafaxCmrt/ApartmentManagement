using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using ApartmentManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Dues.Queries.GetOverdueDues;

public class GetOverdueDuesHandler : IRequestHandler<GetOverdueDuesQuery, Result<PagedResult<OverdueDueDto>>>
{
    private readonly IApplicationDbContext _db;
    private readonly IDateTimeProvider _dateTime;

    public GetOverdueDuesHandler(IApplicationDbContext db, IDateTimeProvider dateTime)
    {
        _db = db;
        _dateTime = dateTime;
    }

    public async Task<Result<PagedResult<OverdueDueDto>>> Handle(GetOverdueDuesQuery request, CancellationToken ct)
    {
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var pageNumber = Math.Max(1, request.PageNumber);
        var today = _dateTime.UtcNow.Date;

        var baseQuery = _db.Dues.AsNoTracking()
            .Where(a => (a.Status == DueStatus.Overdue || a.Status == DueStatus.Pending || a.Status == DueStatus.PartiallyPaid) &&
                        a.DueDate < today);

        var total = await baseQuery.CountAsync(ct);

        var rows = await baseQuery
            .OrderBy(a => a.DueDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new
            {
                a.Id,
                a.ApartmentId,
                ApartmentNumber = a.Apartment != null ? a.Apartment.ApartmentNumber : string.Empty,
                BuildingName = a.Apartment != null && a.Apartment.Building != null ? a.Apartment.Building.Name : null,
                a.Period,
                a.Amount,
                TotalPaid = a.Payments.Sum(o => (decimal?)o.PaidAmount) ?? 0m,
                a.DueDate
            })
            .ToListAsync(ct);

        var items = rows.Select(a => new OverdueDueDto
            {
                Id = a.Id,
                ApartmentId = a.ApartmentId,
                ApartmentNumber = a.ApartmentNumber,
                BuildingName = a.BuildingName,
                Period = a.Period,
                Amount = a.Amount,
                TotalPaid = a.TotalPaid,
                RemainingAmount = a.Amount - a.TotalPaid,
                DueDate = a.DueDate,
                OverdueDays = (today - a.DueDate.Date).Days
            })
            .ToList();

        return Result<PagedResult<OverdueDueDto>>.Success(new PagedResult<OverdueDueDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = total
        });
    }
}
