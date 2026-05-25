using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using ApartmentManagement.Domain.Entities;
using ApartmentManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Meetings.Commands.AddParticipant;

public class AddParticipantHandler : IRequestHandler<AddParticipantCommand, Result<ParticipantDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentTenantService _currentTenant;

    public AddParticipantHandler(IApplicationDbContext db, ICurrentTenantService currentTenant)
    {
        _db = db;
        _currentTenant = currentTenant;
    }

    public async Task<Result<ParticipantDto>> Handle(AddParticipantCommand request, CancellationToken ct)
    {
        if (_currentTenant.TenantId is not { } tenantId)
            return Result<ParticipantDto>.Failure(Error.Unauthorized("Tenant is required."));

        if (!Enum.TryParse<AttendanceStatus>(request.AttendanceStatus, true, out var status))
            return Result<ParticipantDto>.Failure(Error.Validation("Invalid attendance status."));

        var meeting = await _db.Meetings.FirstOrDefaultAsync(t => t.Id == request.MeetingId, ct);
        if (meeting is null)
            return Result<ParticipantDto>.Failure(Error.NotFound("Meeting"));

        var apartment = await _db.Apartments.Include(a => a.Building).FirstOrDefaultAsync(d => d.Id == request.ApartmentId, ct);
        if (apartment is null)
            return Result<ParticipantDto>.Failure(Error.NotFound("Apartment"));

        var dup = await _db.MeetingParticipants
            .AnyAsync(k => k.MeetingId == request.MeetingId && k.ApartmentId == request.ApartmentId, ct);
        if (dup)
            return Result<ParticipantDto>.Failure(Error.Conflict("Bu daire zaten katılımcı listesinde."));

        var participant = new MeetingParticipant
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            MeetingId = request.MeetingId,
            ApartmentId = request.ApartmentId,
            AttendanceStatus = status,
            ProxyApartmentId = request.ProxyApartmentId
        };

        _db.MeetingParticipants.Add(participant);
        try { await _db.SaveChangesAsync(ct); }
        catch (DbUpdateException) { return Result<ParticipantDto>.Failure(Error.Conflict("Bu daire zaten katılımcı listesinde.")); }

        return Result<ParticipantDto>.Success(new ParticipantDto
        {
            Id = participant.Id,
            MeetingId = participant.MeetingId,
            ApartmentId = participant.ApartmentId,
            ApartmentNumber = apartment.ApartmentNumber,
            AttendanceStatus = participant.AttendanceStatus.ToString(),
            ProxyApartmentId = participant.ProxyApartmentId
        });
    }
}
