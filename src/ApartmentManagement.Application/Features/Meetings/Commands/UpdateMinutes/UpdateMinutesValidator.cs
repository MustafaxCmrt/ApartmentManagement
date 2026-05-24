using ApartmentManagement.Application.Common.Validation;
using FluentValidation;

namespace ApartmentManagement.Application.Features.Meetings.Commands.UpdateMinutes;

public class UpdateMinutesValidator : AbstractValidator<UpdateMinutesCommand>
{
    public UpdateMinutesValidator()
    {
        RuleFor(x => x.Id).RequiredGuid();
        RuleFor(x => x.MinutesSummary).NotEmpty().WithMessage(ValidationMessages.Required);
    }
}
