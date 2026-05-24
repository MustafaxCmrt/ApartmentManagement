using ApartmentManagement.Application.Common.Validation;
using ApartmentManagement.Domain.Enums;
using FluentValidation;

namespace ApartmentManagement.Application.Features.Residents.Commands.CreateResident;

public class CreateResidentValidator : AbstractValidator<CreateResidentCommand>
{
    public CreateResidentValidator()
    {
        RuleFor(x => x.ApartmentId).RequiredGuid();
        RuleFor(x => x.FullName).RequiredText(200);
        RuleFor(x => x.Phone).RequiredPhone();
        RuleFor(x => x.Email).OptionalEmail();
        RuleFor(x => x.ResidentType).NotEmpty().WithMessage(ValidationMessages.Required)
            .Must(v => Enum.TryParse<ResidentType>(v, true, out _)).WithMessage("Geçersiz sakin tipi.");
    }
}
