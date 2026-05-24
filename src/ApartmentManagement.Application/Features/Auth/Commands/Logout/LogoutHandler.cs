using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Auth.Commands.Logout;

public class LogoutHandler : IRequestHandler<LogoutCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly IJwtTokenService _jwt;
    private readonly IDateTimeProvider _dateTime;

    public LogoutHandler(IApplicationDbContext db, IJwtTokenService jwt, IDateTimeProvider dateTime)
    {
        _db = db;
        _jwt = jwt;
        _dateTime = dateTime;
    }

    public async Task<Result> Handle(LogoutCommand request, CancellationToken ct)
    {
        var tokenHash = _jwt.HashRefreshToken(request.RefreshToken);

        var rt = await _db.RefreshTokens.FirstOrDefaultAsync(r => r.TokenHash == tokenHash, ct);
        if (rt is null)
            return Result.Success(); // idempotent

        if (rt.RevokedAt is null)
        {
            rt.RevokedAt = _dateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }

        return Result.Success();
    }
}
