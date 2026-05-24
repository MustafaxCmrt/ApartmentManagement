using ApartmentManagement.Application.Common.Validation;
using FluentValidation;

namespace ApartmentManagement.Application.Features.MaintenanceTickets.Commands.DeleteMaintenanceTicket;

public class DeleteMaintenanceTicketValidator : AbstractValidator<DeleteMaintenanceTicketCommand>
{
    public DeleteMaintenanceTicketValidator()
    {
        RuleFor(x => x.Id).RequiredGuid();
    }
}
