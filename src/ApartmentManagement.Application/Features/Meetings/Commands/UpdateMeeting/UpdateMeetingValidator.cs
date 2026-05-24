using FluentValidation;

namespace ApartmentManagement.Application.Features.Meetings.Commands.UpdateMeeting;

public class UpdateMeetingValidator : AbstractValidator<UpdateMeetingCommand>
{
    public UpdateMeetingValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.MeetingDate).NotEmpty();
        RuleFor(x => x.Venue).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Agenda).NotEmpty();
        RuleFor(x => x.Status).NotEmpty();
    }
}
