using ApartmentManagement.Application.Common.DTOs;
using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Domain.Entities;
using ApartmentManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Meetings.Commands.AddParticipantsBatch;

public class AddParticipantsBatchHandler : IRequestHandler<AddParticipantsBatchCommand, Result<List<ParticipantDto>>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentTenantService _currentTenant;

    public AddParticipantsBatchHandler(IApplicationDbContext db, ICurrentTenantService currentTenant)
    {
        _db = db;
        _currentTenant = currentTenant;
    }

    public async Task<Result<List<ParticipantDto>>> Handle(AddParticipantsBatchCommand request, CancellationToken ct)
    {
        if (_currentTenant.TenantId is not { } tenantId)
            return Result<List<ParticipantDto>>.Failure(Error.Unauthorized("Tenant is required."));

        var meeting = await _db.Meetings.FirstOrDefaultAsync(m => m.Id == request.MeetingId, ct);
        if (meeting is null)
            return Result<List<ParticipantDto>>.Failure(Error.NotFound("Meeting"));

        var apartmentIds = request.ApartmentIds.Distinct().ToList();

        var apartments = await _db.Apartments
            .Include(a => a.Building)
            .Where(a => apartmentIds.Contains(a.Id))
            .ToListAsync(ct);

        var existingApartmentIds = await _db.MeetingParticipants
            .Where(p => p.MeetingId == request.MeetingId && apartmentIds.Contains(p.ApartmentId))
            .Select(p => p.ApartmentId)
            .ToListAsync(ct);

        var apartmentsToAdd = apartments.Where(a => !existingApartmentIds.Contains(a.Id)).ToList();

        var newParticipants = apartmentsToAdd.Select(a => new MeetingParticipant
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            MeetingId = request.MeetingId,
            ApartmentId = a.Id,
            AttendanceStatus = AttendanceStatus.Invited
        }).ToList();

        _db.MeetingParticipants.AddRange(newParticipants);
        try { await _db.SaveChangesAsync(ct); }
        catch (DbUpdateException) { return Result<List<ParticipantDto>>.Failure(Error.Conflict("Bazı daireler zaten katılımcı listesinde.")); }

        var apartmentMap = apartmentsToAdd.ToDictionary(a => a.Id);
        var result = newParticipants.Select(p => new ParticipantDto
        {
            Id = p.Id,
            MeetingId = p.MeetingId,
            ApartmentId = p.ApartmentId,
            ApartmentNumber = apartmentMap[p.ApartmentId].ApartmentNumber,
            AttendanceStatus = p.AttendanceStatus.ToString(),
            ProxyApartmentId = null,
            ProxyApartmentNumber = null
        }).ToList();

        return Result<List<ParticipantDto>>.Success(result);
    }
}
