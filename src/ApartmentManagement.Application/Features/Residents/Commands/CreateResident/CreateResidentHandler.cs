using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Utilities;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using ApartmentManagement.Domain.Entities;
using ApartmentManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Residents.Commands.CreateResident;

public class CreateResidentHandler : IRequestHandler<CreateResidentCommand, Result<ResidentDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentTenantService _currentTenant;

    public CreateResidentHandler(IApplicationDbContext db, ICurrentTenantService currentTenant)
    {
        _db = db;
        _currentTenant = currentTenant;
    }

    public async Task<Result<ResidentDto>> Handle(CreateResidentCommand request, CancellationToken ct)
    {
        if (_currentTenant.TenantId is not { } tenantId)
            return Result<ResidentDto>.Failure(Error.Unauthorized("Tenant is required."));

        var apartment = await _db.Apartments.FirstOrDefaultAsync(d => d.Id == request.ApartmentId, ct);
        if (apartment is null)
            return Result<ResidentDto>.Failure(Error.NotFound("Apartment"));

        if (!Enum.TryParse<ResidentType>(request.ResidentType, true, out var residentType))
            return Result<ResidentDto>.Failure(Error.Validation("Invalid resident type."));

        var phone = PhoneNormalizer.Normalize(request.Phone);
        var email = string.IsNullOrWhiteSpace(request.Email) ? null : EmailNormalizer.Normalize(request.Email);

        if (request.IsPrimaryContact)
        {
            var others = await _db.Residents
                .Where(s => s.ApartmentId == request.ApartmentId && s.IsPrimaryContact)
                .ToListAsync(ct);
            foreach (var o in others)
                o.IsPrimaryContact = false;
        }

        var resident = new Resident
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ApartmentId = request.ApartmentId,
            FullName = request.FullName.Trim(),
            Phone = phone,
            Email = email,
            ResidentType = residentType,
            MoveInDate = request.MoveInDate,
            IsPrimaryContact = request.IsPrimaryContact
        };

        _db.Residents.Add(resident);

        var existingActiveTypes = await _db.Residents
            .Where(s => s.ApartmentId == request.ApartmentId && s.MoveOutDate == null)
            .Select(s => s.ResidentType)
            .ToListAsync(ct);

        var allActiveTypes = existingActiveTypes.Concat(new[] { residentType });

        apartment.OccupancyStatus = allActiveTypes.Any(t => t == ResidentType.Owner)
            ? OccupancyStatus.Occupied
            : allActiveTypes.Any(t => t == ResidentType.Tenant)
                ? OccupancyStatus.Rented
                : OccupancyStatus.Vacant;

        await _db.SaveChangesAsync(ct);

        return Result<ResidentDto>.Success(new ResidentDto
        {
            Id = resident.Id,
            TenantId = resident.TenantId,
            ApartmentId = resident.ApartmentId,
            ApartmentNumber = apartment.ApartmentNumber,
            UserId = resident.UserId,
            FullName = resident.FullName,
            Phone = resident.Phone,
            Email = resident.Email,
            ResidentType = resident.ResidentType.ToString(),
            MoveInDate = resident.MoveInDate,
            MoveOutDate = resident.MoveOutDate,
            IsPrimaryContact = resident.IsPrimaryContact,
            HasSystemAccount = false
        });
    }
}
