using ApartmentManagement.Application.Common.Validation;
using FluentValidation;

namespace ApartmentManagement.Application.Features.Meetings.Commands.RemoveParticipant;

public class RemoveParticipantValidator : AbstractValidator<RemoveParticipantCommand>
{
    public RemoveParticipantValidator()
    {
        RuleFor(x => x.ParticipantId).RequiredGuid();
    }
}
