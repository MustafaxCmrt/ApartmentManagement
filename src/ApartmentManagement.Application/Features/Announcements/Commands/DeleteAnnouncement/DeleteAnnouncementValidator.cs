using ApartmentManagement.Application.Common.Validation;
using FluentValidation;

namespace ApartmentManagement.Application.Features.Announcements.Commands.DeleteAnnouncement;

public class DeleteAnnouncementValidator : AbstractValidator<DeleteAnnouncementCommand>
{
    public DeleteAnnouncementValidator()
    {
        RuleFor(x => x.Id).RequiredGuid();
    }
}
