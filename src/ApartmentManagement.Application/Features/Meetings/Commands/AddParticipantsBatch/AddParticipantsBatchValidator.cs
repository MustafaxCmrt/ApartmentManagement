using ApartmentManagement.Application.Common.Validation;
using FluentValidation;

namespace ApartmentManagement.Application.Features.Meetings.Commands.AddParticipantsBatch;

public class AddParticipantsBatchValidator : AbstractValidator<AddParticipantsBatchCommand>
{
    public AddParticipantsBatchValidator()
    {
        RuleFor(x => x.MeetingId).RequiredGuid();
        RuleFor(x => x.ApartmentIds).NotEmpty().WithMessage(ValidationMessages.Required);
    }
}
