using FluentValidation;

namespace ApartmentManagement.Application.Features.Announcements.Commands.CreateAnnouncement;

public class CreateAnnouncementValidator : AbstractValidator<CreateAnnouncementCommand>
{
    public CreateAnnouncementValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Content).NotEmpty();
        RuleFor(x => x.Severity).NotEmpty();
        RuleFor(x => x.Audience).NotEmpty();
        RuleFor(x => x.PublishedAt).NotEmpty();
    }
}
