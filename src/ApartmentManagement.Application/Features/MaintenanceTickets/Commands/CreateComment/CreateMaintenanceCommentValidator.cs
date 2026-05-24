using FluentValidation;

namespace ApartmentManagement.Application.Features.MaintenanceTickets.Commands.CreateComment;

public class CreateMaintenanceCommentValidator : AbstractValidator<CreateMaintenanceCommentCommand>
{
    public CreateMaintenanceCommentValidator()
    {
        RuleFor(x => x.MaintenanceTicketId).NotEmpty();
        RuleFor(x => x.Comment).NotEmpty();
    }
}
