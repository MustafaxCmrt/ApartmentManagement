using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Residents.Queries.GetResidentById;

public class GetResidentByIdHandler : IRequestHandler<GetResidentByIdQuery, Result<ResidentDetailDto>>
{
    private readonly IApplicationDbContext _db;

    public GetResidentByIdHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<ResidentDetailDto>> Handle(GetResidentByIdQuery request, CancellationToken ct)
    {
        var dto = await _db.Residents.AsNoTracking()
            .Where(s => s.Id == request.Id)
            .Select(s => new ResidentDetailDto
            {
                Id = s.Id,
                TenantId = s.TenantId,
                ApartmentId = s.ApartmentId,
                ApartmentNumber = s.Apartment != null ? s.Apartment.ApartmentNumber : null,
                BuildingName = s.Apartment != null && s.Apartment.Building != null ? s.Apartment.Building.Name : null,
                UserId = s.UserId,
                FullName = s.FullName,
                Phone = s.Phone,
                Email = s.Email,
                ResidentType = s.ResidentType.ToString(),
                MoveInDate = s.MoveInDate,
                MoveOutDate = s.MoveOutDate,
                IsPrimaryContact = s.IsPrimaryContact,
                HasSystemAccount = s.UserId != null,
                IsUserActive = s.User != null ? s.User.IsActive : (bool?)null
            })
            .FirstOrDefaultAsync(ct);

        if (dto is null)
            return Result<ResidentDetailDto>.Failure(Error.NotFound("Resident"));

        return Result<ResidentDetailDto>.Success(dto);
    }
}
