using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Utilities;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using ApartmentManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Auth.Commands.Login;

public class LoginHandler : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenService _jwt;
    private readonly IDateTimeProvider _dateTime;
    private readonly ICurrentUserService _currentUser;

    public LoginHandler(
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

    public async Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken ct)
    {
        const string invalidLoginMessage = "Telefon numarası veya şifre hatalı.";

        if (!PhoneNormalizer.TryNormalize(request.Telefon, out var telefon))
            return Result<AuthResponseDto>.Failure(Error.Unauthorized(invalidLoginMessage));

        var user = await _db.Users
            .IgnoreQueryFilters()
            .Where(u => !u.IsDeleted && u.Phone == telefon)
            .OrderBy(u => u.CreatedAt)
            .FirstOrDefaultAsync(ct);

        var validHash = user?.PasswordHash ?? _hasher.DummyHash;
        var passwordOk = _hasher.Verify(request.Sifre, validHash);

        if (user is null || !passwordOk)
            return Result<AuthResponseDto>.Failure(Error.Unauthorized(invalidLoginMessage));

        if (!user.IsActive)
            return Result<AuthResponseDto>.Failure(Error.Unauthorized("Hesap pasif."));

        var (accessToken, expiresAt) = _jwt.GenerateAccessToken(user);
        var refreshTokenPlain = _jwt.GenerateRefreshToken();
        var refreshTokenHash = _jwt.HashRefreshToken(refreshTokenPlain);

        var now = _dateTime.UtcNow;

        var rt = new ApartmentManagement.Domain.Entities.RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = refreshTokenHash,
            ExpiresAt = now.AddDays(_jwt.RefreshTokenDays),
            CreatedAt = now,
            CreatedByIp = _currentUser.IpAddress ?? "unknown"
        };

        _db.RefreshTokens.Add(rt);

        user.LastLoginAt = now;
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
