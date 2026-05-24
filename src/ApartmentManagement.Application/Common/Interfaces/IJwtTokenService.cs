using ApartmentManagement.Domain.Entities;

namespace ApartmentManagement.Application.Common.Interfaces;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAt) GenerateAccessToken(User user);
    string GenerateRefreshToken();
    string HashRefreshToken(string refreshToken);
    int AccessTokenMinutes { get; }
    int RefreshTokenDays { get; }
}
