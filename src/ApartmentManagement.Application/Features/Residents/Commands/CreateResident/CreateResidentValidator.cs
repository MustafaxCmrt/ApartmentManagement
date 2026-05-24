using FluentValidation;

namespace ApartmentManagement.Application.Features.Residents.Commands.CreateResident;

public class CreateResidentValidator : AbstractValidator<CreateResidentCommand>
{
    public CreateResidentValidator()
    {
        RuleFor(x => x.ApartmentId).NotEmpty();
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Email).EmailAddress().MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.ResidentType).NotEmpty();
    }
}
