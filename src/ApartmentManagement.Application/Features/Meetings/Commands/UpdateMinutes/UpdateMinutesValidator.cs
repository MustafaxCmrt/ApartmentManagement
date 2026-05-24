using FluentValidation;

namespace ApartmentManagement.Application.Features.Meetings.Commands.UpdateMinutes;

public class UpdateMinutesValidator : AbstractValidator<UpdateMinutesCommand>
{
    public UpdateMinutesValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.MinutesSummary).NotEmpty();
    }
}
