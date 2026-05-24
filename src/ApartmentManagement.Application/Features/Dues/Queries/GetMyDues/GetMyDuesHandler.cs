using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Dues.Queries.GetMyDues;

public class GetMyDuesHandler : IRequestHandler<GetMyDuesQuery, Result<List<DueDto>>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetMyDuesHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<List<DueDto>>> Handle(GetMyDuesQuery request, CancellationToken ct)
    {
        if (_currentUser.UserId is not { } userId)
            return Result<List<DueDto>>.Failure(Error.Unauthorized());

        var apartmentIds = await _db.Residents.AsNoTracking()
            .Where(s => s.UserId == userId)
            .Select(s => s.ApartmentId)
            .Distinct()
            .ToListAsync(ct);

        if (apartmentIds.Count == 0)
            return Result<List<DueDto>>.Success(new List<DueDto>());

        var list = await _db.Dues.AsNoTracking()
            .Where(a => apartmentIds.Contains(a.ApartmentId))
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
