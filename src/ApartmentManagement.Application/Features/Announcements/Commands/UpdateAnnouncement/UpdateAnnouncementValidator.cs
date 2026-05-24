using FluentValidation;

namespace ApartmentManagement.Application.Features.Announcements.Commands.UpdateAnnouncement;

public class UpdateAnnouncementValidator : AbstractValidator<UpdateAnnouncementCommand>
{
    public UpdateAnnouncementValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Content).NotEmpty();
        RuleFor(x => x.Severity).NotEmpty();
        RuleFor(x => x.Audience).NotEmpty();
        RuleFor(x => x.PublishedAt).NotEmpty();
    }
}
