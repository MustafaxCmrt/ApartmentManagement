namespace ApartmentManagement.Application.Common.Interfaces;

public interface IPasswordHasher
{
    string DummyHash { get; }
    string Hash(string password);
    bool Verify(string password, string hash);
}
