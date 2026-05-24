using ApartmentManagement.Application.Common.Validation;
using ApartmentManagement.Domain.Enums;
using FluentValidation;

namespace ApartmentManagement.Application.Features.Meetings.Commands.UpdateAttendanceStatus;

public class UpdateAttendanceStatusValidator : AbstractValidator<UpdateAttendanceStatusCommand>
{
    public UpdateAttendanceStatusValidator()
    {
        RuleFor(x => x.ParticipantId).RequiredGuid();
        RuleFor(x => x.ProxyApartmentId).NotEqual(Guid.Empty)
            .WithMessage(ValidationMessages.GuidRequired)
            .When(x => x.ProxyApartmentId.HasValue);
        RuleFor(x => x.AttendanceStatus).NotEmpty().WithMessage(ValidationMessages.Required)
            .Must(v => Enum.TryParse<AttendanceStatus>(v, true, out _)).WithMessage("Geçersiz katılım durumu.");
    }
}
