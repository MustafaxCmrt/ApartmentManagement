using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RefreshTokenEntity = ApartmentManagement.Domain.Entities.RefreshToken;

namespace ApartmentManagement.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponseDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly IJwtTokenService _jwt;
    private readonly IDateTimeProvider _dateTime;
    private readonly ICurrentUserService _currentUser;

    public RefreshTokenHandler(
        IApplicationDbContext db,
        IJwtTokenService jwt,
        IDateTimeProvider dateTime,
        ICurrentUserService currentUser)
    {
        _db = db;
        _jwt = jwt;
        _dateTime = dateTime;
        _currentUser = currentUser;
    }

    public async Task<Result<AuthResponseDto>> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var tokenHash = _jwt.HashRefreshToken(request.RefreshToken);
        var now = _dateTime.UtcNow;

        var rt = await _db.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.TokenHash == tokenHash, ct);

        if (rt is null || rt.RevokedAt != null || rt.ExpiresAt < now)
            return Result<AuthResponseDto>.Failure(Error.Unauthorized("Geçersiz veya süresi dolmuş refresh token."));

        var user = rt.User;
        if (user is null || user.IsDeleted || !user.IsActive)
            return Result<AuthResponseDto>.Failure(Error.Unauthorized("Hesap geçersiz."));

        // Rotation: revoke old + create new
        var newRefreshPlain = _jwt.GenerateRefreshToken();
        var newRefreshHash = _jwt.HashRefreshToken(newRefreshPlain);

        rt.RevokedAt = now;
        rt.ReplacedByToken = newRefreshHash;

        var newRt = new RefreshTokenEntity
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = newRefreshHash,
            ExpiresAt = now.AddDays(_jwt.RefreshTokenDays),
            CreatedAt = now,
            CreatedByIp = _currentUser.IpAddress ?? "unknown"
        };

        _db.RefreshTokens.Add(newRt);

        var (accessToken, expiresAt) = _jwt.GenerateAccessToken(user);

        await _db.SaveChangesAsync(ct);

        var response = new AuthResponseDto
        {
            UserId = user.Id,
            TenantId = user.TenantId,
            AccessToken = accessToken,
            RefreshToken = newRefreshPlain,
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
