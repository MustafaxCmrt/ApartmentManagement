using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Utilities;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using ApartmentManagement.Domain.Entities;
using ApartmentManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Auth.Commands.AcceptInvite;

public class AcceptInviteHandler : IRequestHandler<AcceptInviteCommand, Result<AuthResponseDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenService _jwt;
    private readonly IDateTimeProvider _dateTime;
    private readonly ICurrentUserService _currentUser;

    public AcceptInviteHandler(
        IApplicationDbContext db,
        IPasswordHasher hasher,
        IJwtTokenService jwt,
        IDateTimeProvider dateTime,
        ICurrentUserService currentUser)
    {
        _db = db;
        _hasher = hasher;
        _jwt = jwt;
        _dateTime = dateTime;
        _currentUser = currentUser;
    }

    public async Task<Result<AuthResponseDto>> Handle(AcceptInviteCommand request, CancellationToken ct)
    {
        var tokenHash = _jwt.HashRefreshToken(request.InviteToken);
        var now = _dateTime.UtcNow;

        var invite = await _db.UserInvites
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(i => i.TokenHash == tokenHash, ct);

        if (invite is null)
            return Result<AuthResponseDto>.Failure(Error.NotFound("Davet"));

        if (invite.UsedAt != null)
            return Result<AuthResponseDto>.Failure(Error.Conflict("Davet daha önce kullanılmış."));

        if (invite.ExpiresAt < now)
            return Result<AuthResponseDto>.Failure(Error.Conflict("Davetin süresi dolmuş."));

        if (invite.ResidentId is null)
            return Result<AuthResponseDto>.Failure(Error.Validation("Davet sakin kaydı ile ilişkili değil."));

        var sakin = await _db.Residents
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.Id == invite.ResidentId.Value, ct);

        if (sakin is null)
            return Result<AuthResponseDto>.Failure(Error.NotFound("Resident"));

        if (string.IsNullOrWhiteSpace(sakin.Phone))
            return Result<AuthResponseDto>.Failure(Error.Validation("Sakin telefon numarası boş olamaz."));

        var email = EmailNormalizer.Normalize(invite.Email);

        if (!PhoneNormalizer.TryNormalize(sakin.Phone, out var phone))
            return Result<AuthResponseDto>.Failure(Error.Validation("Sakin telefon numarası geçersiz."));

        var emailExists = await _db.Users
            .IgnoreQueryFilters()
            .AnyAsync(u => !u.IsDeleted && u.Email == email, ct);

        if (emailExists)
            return Result<AuthResponseDto>.Failure(Error.Conflict("Bu email zaten kayıtlı."));

        var phoneExists = await _db.Users
            .IgnoreQueryFilters()
            .AnyAsync(u => !u.IsDeleted && u.Phone == phone, ct);

        if (phoneExists)
            return Result<AuthResponseDto>.Failure(Error.Conflict("Bu telefon numarası zaten kayıtlı."));

        var user = new User
        {
            Id = Guid.NewGuid(),
            TenantId = invite.TenantId,
            Email = email,
            PasswordHash = _hasher.Hash(request.Sifre),
            FullName = !string.IsNullOrWhiteSpace(sakin.FullName) ? sakin.FullName : email,
            Phone = phone,
            Role = invite.Role,
            IsActive = true,
            IsEmailVerified = true,
            CreatedAt = now
        };

        _db.Users.Add(user);

        sakin.UserId = user.Id;
        sakin.UpdatedAt = now;

        invite.UsedAt = now;

        var (accessToken, expiresAt) = _jwt.GenerateAccessToken(user);
        var refreshTokenPlain = _jwt.GenerateRefreshToken();
        var refreshTokenHash = _jwt.HashRefreshToken(refreshTokenPlain);

        _db.RefreshTokens.Add(new ApartmentManagement.Domain.Entities.RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = refreshTokenHash,
            ExpiresAt = now.AddDays(_jwt.RefreshTokenDays),
            CreatedAt = now,
            CreatedByIp = _currentUser.IpAddress ?? "unknown"
        });

        await _db.SaveChangesAsync(ct);

        var response = new AuthResponseDto
        {
            UserId = user.Id,
            TenantId = user.TenantId,
            AccessToken = accessToken,
            RefreshToken = refreshTokenPlain,
            ExpiresAt = expiresAt,
            ExpiresIn = _jwt.AccessTokenMinutes * 60,
            User = new LoggedInUserDto
            {
                Id = user.Id,
                TenantId = user.TenantId,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role.ToString()
            }
        };

        return Result<AuthResponseDto>.Success(response);
    }
}
