using ApartmentManagement.Application.Common.Validation;
using ApartmentManagement.Domain.Enums;
using FluentValidation;

namespace ApartmentManagement.Application.Features.Announcements.Commands.UpdateAnnouncement;

public class UpdateAnnouncementValidator : AbstractValidator<UpdateAnnouncementCommand>
{
    public UpdateAnnouncementValidator()
    {
        RuleFor(x => x.Id).RequiredGuid();
        RuleFor(x => x.Title).RequiredText(200);
        RuleFor(x => x.Content).NotEmpty().WithMessage(ValidationMessages.Required);
        RuleFor(x => x.Severity).NotEmpty().WithMessage(ValidationMessages.Required)
            .Must(v => Enum.TryParse<AnnouncementSeverity>(v, true, out _)).WithMessage("Geçersiz duyuru önceliği.");
        RuleFor(x => x.Audience).NotEmpty().WithMessage(ValidationMessages.Required)
            .Must(v => Enum.TryParse<AnnouncementAudience>(v, true, out _)).WithMessage("Geçersiz duyuru hedef kitlesi.");
        RuleFor(x => x.PublishedAt).NotEmpty().WithMessage(ValidationMessages.Required);
        RuleFor(x => x.BuildingId).NotEqual(Guid.Empty)
            .WithMessage(ValidationMessages.GuidRequired)
            .When(x => x.BuildingId.HasValue);
    }
}
