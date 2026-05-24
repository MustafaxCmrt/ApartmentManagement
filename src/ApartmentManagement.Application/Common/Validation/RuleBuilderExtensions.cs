using System.ComponentModel.DataAnnotations;
using ApartmentManagement.Application.Common.Utilities;
using FluentValidation;

namespace ApartmentManagement.Application.Common.Validation;

public static class RuleBuilderExtensions
{
    private static readonly EmailAddressAttribute EmailValidator = new();

    public static IRuleBuilderOptions<T, string> RequiredText<T>(
        this IRuleBuilder<T, string> rule,
        int maxLength)
        => rule.NotEmpty().WithMessage(ValidationMessages.Required)
            .MaximumLength(maxLength).WithMessage(ValidationMessages.MaxLength);

    public static IRuleBuilderOptions<T, Guid> RequiredGuid<T>(
        this IRuleBuilder<T, Guid> rule)
        => rule.NotEqual(Guid.Empty).WithMessage(ValidationMessages.GuidRequired);

    public static IRuleBuilderOptions<T, string> RequiredEmail<T>(
        this IRuleBuilder<T, string> rule)
        => rule.NotEmpty().WithMessage(ValidationMessages.Required)
            .EmailAddress().WithMessage(ValidationMessages.EmailInvalid)
            .MaximumLength(200).WithMessage(ValidationMessages.MaxLength);

    public static IRuleBuilderOptions<T, string?> OptionalEmail<T>(
        this IRuleBuilder<T, string?> rule)
        => rule.MaximumLength(200).WithMessage(ValidationMessages.MaxLength)
            .Must(v => string.IsNullOrWhiteSpace(v) || EmailValidator.IsValid(v))
            .WithMessage(ValidationMessages.EmailInvalid);

    public static IRuleBuilderOptions<T, string> RequiredPhone<T>(
        this IRuleBuilder<T, string> rule)
        => rule.NotEmpty().WithMessage(ValidationMessages.Required)
            .MaximumLength(30).WithMessage(ValidationMessages.MaxLength)
            .Must(v => PhoneNormalizer.TryNormalize(v, out _))
            .WithMessage(ValidationMessages.PhoneInvalid);

    public static IRuleBuilderOptions<T, string?> OptionalPhone<T>(
        this IRuleBuilder<T, string?> rule)
        => rule.MaximumLength(30).WithMessage(ValidationMessages.MaxLength)
            .Must(v => string.IsNullOrWhiteSpace(v) || PhoneNormalizer.TryNormalize(v, out _))
            .WithMessage(ValidationMessages.PhoneInvalid);

    public static IRuleBuilderOptions<T, string> StrongPassword<T>(
        this IRuleBuilder<T, string> rule)
        => rule.NotEmpty().WithMessage(ValidationMessages.Required)
            .MinimumLength(8).WithMessage(ValidationMessages.PasswordWeak)
            .MaximumLength(100).WithMessage(ValidationMessages.MaxLength)
            .Matches(@"^(?=.*[A-Za-z])(?=.*\d).+$").WithMessage(ValidationMessages.PasswordWeak);
}
