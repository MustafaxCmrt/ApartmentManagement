using FluentValidation;

namespace ApartmentManagement.Application.Features.MaintenanceTickets.Commands.CreateMaintenanceTicket;

public class CreateMaintenanceTicketValidator : AbstractValidator<CreateMaintenanceTicketCommand>
{
    public CreateMaintenanceTicketValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Location).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Priority).NotEmpty();
    }
}
