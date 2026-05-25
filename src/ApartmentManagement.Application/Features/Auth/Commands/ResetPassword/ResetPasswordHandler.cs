using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, Result>
{
    private const string InvalidTokenMessage = "Geçersiz veya süresi dolmuş şifre sıfırlama bağlantısı.";

    private readonly IApplicationDbContext _db;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenService _jwt;
    private readonly IDateTimeProvider _dateTime;

    public ResetPasswordHandler(
        IApplicationDbContext db,
        IPasswordHasher hasher,
        IJwtTokenService jwt,
        IDateTimeProvider dateTime)
    {
        _db = db;
        _hasher = hasher;
        _jwt = jwt;
        _dateTime = dateTime;
    }

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken ct)
    {
        var tokenHash = _jwt.HashRefreshToken(request.Token);
        var now = _dateTime.UtcNow;

        var resetToken = await _db.PasswordResetTokens
            .IgnoreQueryFilters()
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, ct);

        if (resetToken is null || resetToken.IsUsed || resetToken.ExpiresAt < now)
            return Result.Failure(Error.Unauthorized(InvalidTokenMessage));

        var user = resetToken.User;
        if (user is null || user.IsDeleted || !user.IsActive)
            return Result.Failure(Error.Unauthorized(InvalidTokenMessage));

        user.PasswordHash = _hasher.Hash(request.NewPassword);
        resetToken.IsUsed = true;
        resetToken.UsedAt = now;

        await _db.RefreshTokens
            .Where(rt => rt.UserId == user.Id && rt.RevokedAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(rt => rt.RevokedAt, now), ct);

        await _db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
