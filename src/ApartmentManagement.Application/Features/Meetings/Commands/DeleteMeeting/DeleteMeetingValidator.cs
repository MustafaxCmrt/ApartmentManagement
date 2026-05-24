using ApartmentManagement.Application.Common.Validation;
using FluentValidation;

namespace ApartmentManagement.Application.Features.Meetings.Commands.DeleteMeeting;

public class DeleteMeetingValidator : AbstractValidator<DeleteMeetingCommand>
{
    public DeleteMeetingValidator()
    {
        RuleFor(x => x.Id).RequiredGuid();
    }
}
