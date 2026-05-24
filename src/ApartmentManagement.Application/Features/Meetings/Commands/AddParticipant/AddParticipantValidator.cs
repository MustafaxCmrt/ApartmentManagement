using ApartmentManagement.Application.Common.Validation;
using ApartmentManagement.Domain.Enums;
using FluentValidation;

namespace ApartmentManagement.Application.Features.Meetings.Commands.AddParticipant;

public class AddParticipantValidator : AbstractValidator<AddParticipantCommand>
{
    public AddParticipantValidator()
    {
        RuleFor(x => x.MeetingId).RequiredGuid();
        RuleFor(x => x.ApartmentId).RequiredGuid();
        RuleFor(x => x.ProxyApartmentId).NotEqual(Guid.Empty)
            .WithMessage(ValidationMessages.GuidRequired)
            .When(x => x.ProxyApartmentId.HasValue);
        RuleFor(x => x.AttendanceStatus).NotEmpty().WithMessage(ValidationMessages.Required)
            .Must(v => Enum.TryParse<AttendanceStatus>(v, true, out _)).WithMessage("Geçersiz katılım durumu.");
    }
}
