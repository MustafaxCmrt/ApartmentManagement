using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using ApartmentManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Residents.Queries.GetAllResidents;

public class GetAllResidentsHandler : IRequestHandler<GetAllResidentsQuery, Result<PagedResult<ResidentDto>>>
{
    private readonly IApplicationDbContext _db;

    public GetAllResidentsHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<PagedResult<ResidentDto>>> Handle(GetAllResidentsQuery request, CancellationToken ct)
    {
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var pageNumber = Math.Max(1, request.PageNumber);

        var query = _db.Residents.AsNoTracking();

        if (request.ApartmentId.HasValue)
            query = query.Where(s => s.ApartmentId == request.ApartmentId.Value);

        if (!string.IsNullOrWhiteSpace(request.ResidentType) &&
            Enum.TryParse<ResidentType>(request.ResidentType, true, out var type))
            query = query.Where(s => s.ResidentType == type);

        if (request.IsActive.HasValue)
        {
            if (request.IsActive.Value) query = query.Where(s => s.MoveOutDate == null);
            else query = query.Where(s => s.MoveOutDate != null);
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderBy(s => s.FullName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new ResidentDto
            {
                Id = s.Id,
                TenantId = s.TenantId,
                ApartmentId = s.ApartmentId,
                ApartmentNumber = s.Apartment != null ? s.Apartment.ApartmentNumber : null,
                UserId = s.UserId,
                FullName = s.FullName,
                Phone = s.Phone,
                Email = s.Email,
                ResidentType = s.ResidentType.ToString(),
                MoveInDate = s.MoveInDate,
                MoveOutDate = s.MoveOutDate,
                IsPrimaryContact = s.IsPrimaryContact,
                HasSystemAccount = s.UserId != null
            })
            .ToListAsync(ct);

        return Result<PagedResult<ResidentDto>>.Success(new PagedResult<ResidentDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = total
        });
    }
}
