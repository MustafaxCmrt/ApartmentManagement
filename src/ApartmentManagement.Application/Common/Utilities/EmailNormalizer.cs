namespace ApartmentManagement.Application.Common.Utilities;

public static class EmailNormalizer
{
    public static string Normalize(string? raw)
        => (raw ?? string.Empty).Trim().ToLowerInvariant();
}
