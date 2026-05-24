using ApartmentManagement.Application.Common.Validation;
using ApartmentManagement.Domain.Enums;
using FluentValidation;

namespace ApartmentManagement.Application.Features.Meetings.Commands.UpdateMeeting;

public class UpdateMeetingValidator : AbstractValidator<UpdateMeetingCommand>
{
    public UpdateMeetingValidator()
    {
        RuleFor(x => x.Id).RequiredGuid();
        RuleFor(x => x.Title).RequiredText(200);
        RuleFor(x => x.MeetingDate).NotEmpty().WithMessage(ValidationMessages.Required);
        RuleFor(x => x.Venue).RequiredText(200);
        RuleFor(x => x.Agenda).NotEmpty().WithMessage(ValidationMessages.Required);
        RuleFor(x => x.Status).NotEmpty().WithMessage(ValidationMessages.Required)
            .Must(v => Enum.TryParse<MeetingStatus>(v, true, out _)).WithMessage("Geçersiz toplantı durumu.");
    }
}
