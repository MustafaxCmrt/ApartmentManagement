using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.Utilities;
using ApartmentManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly IJwtTokenService _jwt;
    private readonly IEmailService _email;
    private readonly IDateTimeProvider _dateTime;

    public ForgotPasswordHandler(
        IApplicationDbContext db,
        IJwtTokenService jwt,
        IEmailService email,
        IDateTimeProvider dateTime)
    {
        _db = db;
        _jwt = jwt;
        _email = email;
        _dateTime = dateTime;
    }

    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken ct)
    {
        var email = EmailNormalizer.Normalize(request.Email);

        var user = await _db.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => !u.IsDeleted && u.IsActive && u.Email == email, ct);

        if (user is not null)
        {
            var plain = _jwt.GenerateRefreshToken();
            var hash = _jwt.HashRefreshToken(plain);
            var now = _dateTime.UtcNow;

            _db.PasswordResetTokens.Add(new PasswordResetToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TokenHash = hash,
                ExpiresAt = now.AddMinutes(60),
                CreatedAt = now,
                IsUsed = false,
                UsedAt = null
            });

            await _db.SaveChangesAsync(ct);

            var resetUrl = $"/reset-password?token={plain}";
            await _email.SendPasswordResetEmailAsync(user.Email, resetUrl, ct);
        }

        return Result.Success();
    }
}
