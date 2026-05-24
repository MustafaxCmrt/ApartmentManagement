using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Apartments.Queries.GetApartmentResidents;

public class GetApartmentResidentsHandler : IRequestHandler<GetApartmentResidentsQuery, Result<List<ResidentDto>>>
{
    private readonly IApplicationDbContext _db;

    public GetApartmentResidentsHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<List<ResidentDto>>> Handle(GetApartmentResidentsQuery request, CancellationToken ct)
    {
        var apartmentExists = await _db.Apartments.AnyAsync(d => d.Id == request.ApartmentId, ct);
        if (!apartmentExists)
            return Result<List<ResidentDto>>.Failure(Error.NotFound("Apartment"));

        var residents = await _db.Residents.AsNoTracking()
            .Where(s => s.ApartmentId == request.ApartmentId)
            .OrderByDescending(s => s.IsPrimaryContact)
            .ThenBy(s => s.FullName)
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

        return Result<List<ResidentDto>>.Success(residents);
    }
}
