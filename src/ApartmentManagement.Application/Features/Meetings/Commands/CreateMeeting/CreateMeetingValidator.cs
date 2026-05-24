using ApartmentManagement.Application.Common.Validation;
using FluentValidation;

namespace ApartmentManagement.Application.Features.Meetings.Commands.CreateMeeting;

public class CreateMeetingValidator : AbstractValidator<CreateMeetingCommand>
{
    public CreateMeetingValidator()
    {
        RuleFor(x => x.Title).RequiredText(200);
        RuleFor(x => x.MeetingDate).NotEmpty().WithMessage(ValidationMessages.Required);
        RuleFor(x => x.Venue).RequiredText(200);
        RuleFor(x => x.Agenda).NotEmpty().WithMessage(ValidationMessages.Required);
    }
}
