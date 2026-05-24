using FluentValidation;

namespace ApartmentManagement.Application.Features.MaintenanceTickets.Commands.UpdateMaintenanceTicket;

public class UpdateMaintenanceTicketValidator : AbstractValidator<UpdateMaintenanceTicketCommand>
{
    public UpdateMaintenanceTicketValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Location).NotEmpty().MaximumLength(200);
        RuleFor(x => x.AssignedTo).MaximumLength(200);
    }
}
