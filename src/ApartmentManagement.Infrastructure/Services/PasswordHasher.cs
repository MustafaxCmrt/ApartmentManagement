using ApartmentManagement.Application.Common.Interfaces;

namespace ApartmentManagement.Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
    public string DummyHash { get; } = BCrypt.Net.BCrypt.HashPassword("dummy-password-for-timing", workFactor: 12);

    public string Hash(string password)
        => BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);

    public bool Verify(string password, string hash)
        => BCrypt.Net.BCrypt.Verify(password, hash);
}
