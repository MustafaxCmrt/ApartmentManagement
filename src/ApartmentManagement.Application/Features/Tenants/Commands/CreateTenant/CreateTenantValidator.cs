using FluentValidation;

namespace ApartmentManagement.Application.Features.Tenants.Commands.CreateTenant;

public class CreateTenantValidator : AbstractValidator<CreateTenantCommand>
{
    public CreateTenantValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ShortName).NotEmpty().MaximumLength(50)
            .Matches("^[a-zA-Z0-9_-]+$").WithMessage("Short name can only contain letters, numbers, underscores and dashes.");
        RuleFor(x => x.ContactEmail).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.ContactPhone).MaximumLength(30);
        RuleFor(x => x.Address).MaximumLength(500);
        RuleFor(x => x.MaxApartmentCount).GreaterThan(0).LessThanOrEqualTo(10000);
    }
}
