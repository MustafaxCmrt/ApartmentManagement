using ApartmentManagement.Application.Common.Validation;
using FluentValidation;

namespace ApartmentManagement.Application.Features.Tenants.Commands.CreateTenant;

public class CreateTenantValidator : AbstractValidator<CreateTenantCommand>
{
    public CreateTenantValidator()
    {
        RuleFor(x => x.Name).RequiredText(200);
        RuleFor(x => x.ShortName).RequiredText(50)
            .Matches("^[a-zA-Z0-9_-]+$").WithMessage("Kısa ad sadece harf, rakam, alt çizgi ve tire içerebilir.");
        RuleFor(x => x.ContactEmail).RequiredEmail();
        RuleFor(x => x.ContactPhone).OptionalPhone();
        RuleFor(x => x.Address).MaximumLength(500).WithMessage(ValidationMessages.MaxLength);
        RuleFor(x => x.MaxApartmentCount).GreaterThan(0).WithMessage(ValidationMessages.AmountPositive)
            .LessThanOrEqualTo(10000).WithMessage("{PropertyName} en fazla 10000 olabilir.");
    }
}
