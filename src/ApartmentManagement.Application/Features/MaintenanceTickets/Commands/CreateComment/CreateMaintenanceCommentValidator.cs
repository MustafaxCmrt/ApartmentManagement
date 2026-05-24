using ApartmentManagement.Application.Common.Validation;
using FluentValidation;

namespace ApartmentManagement.Application.Features.MaintenanceTickets.Commands.CreateComment;

public class CreateMaintenanceCommentValidator : AbstractValidator<CreateMaintenanceCommentCommand>
{
    public CreateMaintenanceCommentValidator()
    {
        RuleFor(x => x.MaintenanceTicketId).RequiredGuid();
        RuleFor(x => x.Comment).NotEmpty().WithMessage(ValidationMessages.Required);
    }
}
