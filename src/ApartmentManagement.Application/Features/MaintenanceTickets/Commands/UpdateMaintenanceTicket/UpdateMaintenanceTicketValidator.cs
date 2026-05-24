using ApartmentManagement.Application.Common.Validation;
using FluentValidation;

namespace ApartmentManagement.Application.Features.MaintenanceTickets.Commands.UpdateMaintenanceTicket;

public class UpdateMaintenanceTicketValidator : AbstractValidator<UpdateMaintenanceTicketCommand>
{
    public UpdateMaintenanceTicketValidator()
    {
        RuleFor(x => x.Id).RequiredGuid();
        RuleFor(x => x.Title).RequiredText(200);
        RuleFor(x => x.Description).NotEmpty().WithMessage(ValidationMessages.Required);
        RuleFor(x => x.Location).RequiredText(200);
        RuleFor(x => x.AssignedTo).MaximumLength(200).WithMessage(ValidationMessages.MaxLength);
        RuleFor(x => x.EstimatedCost).GreaterThanOrEqualTo(0).WithMessage("{PropertyName} negatif olamaz.")
            .When(x => x.EstimatedCost.HasValue);
        RuleFor(x => x.ActualCost).GreaterThanOrEqualTo(0).WithMessage("{PropertyName} negatif olamaz.")
            .When(x => x.ActualCost.HasValue);
    }
}
