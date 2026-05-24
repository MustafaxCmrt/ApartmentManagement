using ApartmentManagement.Application.Common.Validation;
using FluentValidation;

namespace ApartmentManagement.Application.Features.Tenants.Commands.UpdateTenant;

public class UpdateTenantValidator : AbstractValidator<UpdateTenantCommand>
{
    public UpdateTenantValidator()
    {
        RuleFor(x => x.Id).RequiredGuid();
        RuleFor(x => x.Name).RequiredText(200);
        RuleFor(x => x.ContactEmail).RequiredEmail();
        RuleFor(x => x.ContactPhone).OptionalPhone();
        RuleFor(x => x.Address).MaximumLength(500).WithMessage(ValidationMessages.MaxLength);
        RuleFor(x => x.MaxApartmentCount).GreaterThan(0).WithMessage(ValidationMessages.AmountPositive)
            .LessThanOrEqualTo(10000).WithMessage("{PropertyName} en fazla 10000 olabilir.");
    }
}
