using System.Text.RegularExpressions;

namespace ApartmentManagement.Application.Common.Utilities;

public static class PhoneNormalizer
{
    private static readonly Regex DigitsOnly = new(@"[^\d]", RegexOptions.Compiled);

    public static bool TryNormalize(string? raw, out string normalized)
    {
        normalized = string.Empty;
        if (string.IsNullOrWhiteSpace(raw))
            return false;

        var digits = DigitsOnly.Replace(raw, "");

        if (digits.Length == 12 && digits.StartsWith("90"))
            digits = digits[2..];
        else if (digits.Length == 11 && digits.StartsWith("0"))
            digits = digits[1..];
        else if (digits.Length != 10)
            return false;

        if (digits[0] is not ('2' or '3' or '4' or '5'))
            return false;

        normalized = "+90" + digits;
        return true;
    }

    public static string Normalize(string? raw)
    {
        if (!TryNormalize(raw, out var normalized))
            throw new FormatException("Geçersiz telefon numarası.");

        return normalized;
    }
}
