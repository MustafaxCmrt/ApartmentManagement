namespace ApartmentManagement.Application.Common.Validation;

public static class ValidationMessages
{
    public const string Required = "{PropertyName} alanı zorunludur.";
    public const string MaxLength = "{PropertyName} en fazla {MaxLength} karakter olabilir.";
    public const string MinLength = "{PropertyName} en az {MinLength} karakter olmalıdır.";
    public const string EmailInvalid = "Geçerli bir e-posta adresi giriniz.";
    public const string PhoneInvalid = "Telefon numarası geçersiz. Örnek: +905551112233 veya 05551112233.";
    public const string GuidRequired = "{PropertyName} geçerli bir kimlik (Guid) olmalıdır.";
    public const string PasswordWeak = "Şifre en az 8 karakter olmalı, en az 1 harf ve 1 rakam içermelidir.";
    public const string PasswordsDoNotMatch = "Şifreler eşleşmiyor.";
    public const string NewPasswordSameAsOld = "Yeni şifre eski şifre ile aynı olamaz.";
    public const string DateInPast = "{PropertyName} geçmiş bir tarih olamaz.";
    public const string AmountPositive = "{PropertyName} pozitif olmalıdır.";
}
