using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using ApartmentManagement.Domain.Entities;
using ApartmentManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Residents.Commands.CreateInvite;

public class CreateInviteHandler : IRequestHandler<CreateInviteCommand, Result<InviteResponseDto>>
{
    private const int InviteExpiryDays = 3;

    private readonly IApplicationDbContext _db;
    private readonly IJwtTokenService _jwt;
    private readonly IDateTimeProvider _dateTime;
    private readonly ICurrentUserService _currentUser;
    private readonly ICurrentTenantService _currentTenant;

    public CreateInviteHandler(
        IApplicationDbContext db,
        IJwtTokenService jwt,
        IDateTimeProvider dateTime,
        ICurrentUserService currentUser,
        ICurrentTenantService currentTenant)
    {
        _db = db;
        _jwt = jwt;
        _dateTime = dateTime;
        _currentUser = currentUser;
        _currentTenant = currentTenant;
    }

    public async Task<Result<InviteResponseDto>> Handle(CreateInviteCommand request, CancellationToken ct)
    {
        if (_currentTenant.TenantId is not { } tenantId || _currentUser.UserId is not { } userId)
            return Result<InviteResponseDto>.Failure(Error.Unauthorized());

        var resident = await _db.Residents.FirstOrDefaultAsync(s => s.Id == request.ResidentId, ct);
        if (resident is null)
            return Result<InviteResponseDto>.Failure(Error.NotFound("Resident"));

        if (resident.UserId != null)
            return Result<InviteResponseDto>.Failure(Error.Conflict("This resident is already registered in the system."));

        if (string.IsNullOrWhiteSpace(resident.Email))
            return Result<InviteResponseDto>.Failure(Error.Validation("The resident must have an email address to receive an invitation."));

        var tokenPlain = _jwt.GenerateRefreshToken(); // random secure token
        var tokenHash = _jwt.HashRefreshToken(tokenPlain);
        var now = _dateTime.UtcNow;
        var expiresAt = now.AddDays(InviteExpiryDays);

        var invite = new UserInvite
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ResidentId = resident.Id,
            Email = resident.Email,
            TokenHash = tokenHash,
            Role = UserRole.Resident,
            CreatedAt = now,
            ExpiresAt = expiresAt,
            CreatedBy = userId
        };

        _db.UserInvites.Add(invite);
        await _db.SaveChangesAsync(ct);

        var inviteUrl = $"/accept-invite?token={tokenPlain}";

        return Result<InviteResponseDto>.Success(new InviteResponseDto
        {
            InviteId = invite.Id,
            InviteToken = tokenPlain,
            InviteUrl = inviteUrl,
            ExpiresAt = expiresAt
        });
    }
}
