using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using ApartmentManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Dues.Queries.GetAllDues;

public class GetAllDuesHandler : IRequestHandler<GetAllDuesQuery, Result<PagedResult<DueDto>>>
{
    private readonly IApplicationDbContext _db;

    public GetAllDuesHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<PagedResult<DueDto>>> Handle(GetAllDuesQuery request, CancellationToken ct)
    {
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var pageNumber = Math.Max(1, request.PageNumber);

        var query = _db.Dues.AsNoTracking();

        if (request.Period.HasValue)
        {
            var period = new DateTime(request.Period.Value.Year, request.Period.Value.Month, 1);
            query = query.Where(a => a.Period.Year == period.Year && a.Period.Month == period.Month);
        }

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<DueStatus>(request.Status, true, out var status))
            query = query.Where(a => a.Status == status);

        if (request.BuildingId.HasValue)
            query = query.Where(a => a.Apartment != null && a.Apartment.BuildingId == request.BuildingId.Value);

        if (request.ApartmentId.HasValue)
            query = query.Where(a => a.ApartmentId == request.ApartmentId.Value);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(a => a.Period)
            .ThenBy(a => a.ApartmentId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
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

        return Result<PagedResult<DueDto>>.Success(new PagedResult<DueDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = total
        });
    }
}
