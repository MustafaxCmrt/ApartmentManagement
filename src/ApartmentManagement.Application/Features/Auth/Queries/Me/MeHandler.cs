using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Auth.Queries.Me;

public class MeHandler : IRequestHandler<MeQuery, Result<UserDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public MeHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<UserDto>> Handle(MeQuery request, CancellationToken ct)
    {
        if (_currentUser.UserId is not { } userId)
            return Result<UserDto>.Failure(Error.Unauthorized());

        var dto = await _db.Users
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(u => u.Id == userId && !u.IsDeleted)
            .Select(u => new UserDto
            {
                Id = u.Id,
                TenantId = u.TenantId,
                Email = u.Email,
                FullName = u.FullName,
                Phone = u.Phone,
                Role = u.Role.ToString(),
                IsActive = u.IsActive,
                LastLoginAt = u.LastLoginAt,
                IsEmailVerified = u.IsEmailVerified
            })
            .FirstOrDefaultAsync(ct);

        if (dto is null)
            return Result<UserDto>.Failure(Error.NotFound("Kullanıcı"));

        return Result<UserDto>.Success(dto);
    }
}
