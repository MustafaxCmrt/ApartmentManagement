using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using ApartmentManagement.Domain.Entities;
using ApartmentManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Auth.Commands.RegisterAdmin;

public class RegisterAdminHandler : IRequestHandler<RegisterAdminCommand, Result<AuthResponseDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenService _jwt;
    private readonly IDateTimeProvider _dateTime;
    private readonly ICurrentUserService _currentUser;

    public RegisterAdminHandler(
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

    public async Task<Result<AuthResponseDto>> Handle(RegisterAdminCommand request, CancellationToken ct)
    {
        var kisaAd = request.ApartmanKisaAd.Trim().ToLowerInvariant();
        var adminEmail = request.AdminEmail.Trim();
        var adminTelefon = request.AdminTelefon.Trim();

        var kisaAdExists = await _db.Tenants
            .AnyAsync(t => t.ShortName == kisaAd, ct);

        if (kisaAdExists)
            return Result<AuthResponseDto>.Failure(Error.Conflict("Bu kısa ad zaten kullanılıyor."));

        var emailExists = await _db.Users
            .IgnoreQueryFilters()
            .AnyAsync(u => !u.IsDeleted && u.Email == adminEmail, ct);

        if (emailExists)
            return Result<AuthResponseDto>.Failure(Error.Conflict("Bu email zaten kayıtlı."));

        var phoneExists = await _db.Users
            .IgnoreQueryFilters()
            .AnyAsync(u => !u.IsDeleted && u.Phone == adminTelefon, ct);

        if (phoneExists)
            return Result<AuthResponseDto>.Failure(Error.Conflict("Bu telefon numarası zaten kayıtlı."));

        var now = _dateTime.UtcNow;

        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = request.ApartmanAdi.Trim(),
            ShortName = kisaAd,
            IsActive = true,
            SubscriptionStart = now,
            SubscriptionEnd = null,
            MaxApartmentCount = 50,
            ContactEmail = request.ContactEmail.Trim(),
            ContactPhone = request.ContactPhone?.Trim(),
            CreatedAt = now,
            UpdatedAt = now
        };

        _db.Tenants.Add(tenant);

        var user = new User
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.Id,
            Email = adminEmail,
            PasswordHash = _hasher.Hash(request.Sifre),
            FullName = request.AdminAdSoyad.Trim(),
            Phone = adminTelefon,
            Role = UserRole.TenantAdmin,
            IsActive = true,
            IsEmailVerified = false,
            CreatedAt = now
        };

        _db.Users.Add(user);

        var (accessToken, expiresAt) = _jwt.GenerateAccessToken(user);
        var refreshTokenPlain = _jwt.GenerateRefreshToken();
        var refreshTokenHash = _jwt.HashRefreshToken(refreshTokenPlain);

        var refreshToken = new ApartmentManagement.Domain.Entities.RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = refreshTokenHash,
            ExpiresAt = now.AddDays(_jwt.RefreshTokenDays),
            CreatedAt = now,
            CreatedByIp = _currentUser.IpAddress ?? "unknown"
        };

        _db.RefreshTokens.Add(refreshToken);

        await _db.SaveChangesAsync(ct);

        var response = new AuthResponseDto
        {
            UserId = user.Id,
            TenantId = tenant.Id,
            AccessToken = accessToken,
            RefreshToken = refreshTokenPlain,
            ExpiresAt = expiresAt,
            ExpiresIn = _jwt.AccessTokenMinutes * 60,
            User = new LoggedInUserDto
            {
                Id = user.Id,
                TenantId = tenant.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role.ToString()
            }
        };

        return Result<AuthResponseDto>.Success(response);
    }
}
