using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Auth.Commands.ChangePassword;

public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly IPasswordHasher _hasher;
    private readonly IDateTimeProvider _dateTime;
    private readonly ICurrentUserService _currentUser;

    public ChangePasswordHandler(
        IApplicationDbContext db,
        IPasswordHasher hasher,
        IDateTimeProvider dateTime,
        ICurrentUserService currentUser)
    {
        _db = db;
        _hasher = hasher;
        _dateTime = dateTime;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken ct)
    {
        if (_currentUser.UserId is not { } userId)
            return Result.Failure(Error.Unauthorized());

        var user = await _db.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, ct);

        if (user is null)
            return Result.Failure(Error.NotFound("Kullanıcı"));

        if (!_hasher.Verify(request.EskiSifre, user.PasswordHash))
            return Result.Failure(Error.Unauthorized("Eski şifre hatalı."));

        user.PasswordHash = _hasher.Hash(request.YeniSifre);
        user.UpdatedAt = _dateTime.UtcNow;
        user.UpdatedBy = userId;

        await _db.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(rt => rt.RevokedAt, _dateTime.UtcNow), ct);

        await _db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
