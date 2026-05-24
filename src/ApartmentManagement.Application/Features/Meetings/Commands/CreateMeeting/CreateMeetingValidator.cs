using FluentValidation;

namespace ApartmentManagement.Application.Features.Meetings.Commands.CreateMeeting;

public class CreateMeetingValidator : AbstractValidator<CreateMeetingCommand>
{
    public CreateMeetingValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.MeetingDate).NotEmpty();
        RuleFor(x => x.Venue).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Agenda).NotEmpty();
    }
}
