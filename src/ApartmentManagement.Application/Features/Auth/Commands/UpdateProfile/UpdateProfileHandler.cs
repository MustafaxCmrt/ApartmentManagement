using ApartmentManagement.Application.Common.DTOs;
using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Auth.Commands.UpdateProfile;

public class UpdateProfileHandler : IRequestHandler<UpdateProfileCommand, Result<UserDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public UpdateProfileHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<UserDto>> Handle(UpdateProfileCommand request, CancellationToken ct)
    {
        if (_currentUser.UserId is not { } userId)
            return Result<UserDto>.Failure(Error.Unauthorized());

        var user = await _db.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, ct);

        if (user is null)
            return Result<UserDto>.Failure(Error.NotFound("Kullanıcı"));

        var newEmail = EmailNormalizer.Normalize(request.Email);

        if (user.Email != newEmail)
        {
            var emailExists = await _db.Users
                .IgnoreQueryFilters()
                .AnyAsync(u => !u.IsDeleted && u.Id != userId && u.Email == newEmail, ct);

            if (emailExists)
                return Result<UserDto>.Failure(Error.Conflict("Bu e-posta zaten kullanılıyor."));

            user.Email = newEmail;
            user.IsEmailVerified = false;
        }

        user.FullName = request.FullName.Trim();
        user.Phone = PhoneNormalizer.Normalize(request.Phone);

        await _db.SaveChangesAsync(ct);

        return Result<UserDto>.Success(new UserDto
        {
            Id = user.Id,
            TenantId = user.TenantId,
            Email = user.Email,
            FullName = user.FullName,
            Phone = user.Phone,
            Role = user.Role.ToString(),
            IsActive = user.IsActive,
            LastLoginAt = user.LastLoginAt,
            IsEmailVerified = user.IsEmailVerified
        });
    }
}
