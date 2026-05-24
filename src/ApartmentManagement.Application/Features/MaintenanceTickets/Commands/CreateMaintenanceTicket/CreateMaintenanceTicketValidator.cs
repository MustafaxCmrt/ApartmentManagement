using ApartmentManagement.Application.Common.Validation;
using ApartmentManagement.Domain.Enums;
using FluentValidation;

namespace ApartmentManagement.Application.Features.MaintenanceTickets.Commands.CreateMaintenanceTicket;

public class CreateMaintenanceTicketValidator : AbstractValidator<CreateMaintenanceTicketCommand>
{
    public CreateMaintenanceTicketValidator()
    {
        RuleFor(x => x.ApartmentId).NotEqual(Guid.Empty)
            .WithMessage(ValidationMessages.GuidRequired)
            .When(x => x.ApartmentId.HasValue);
        RuleFor(x => x.Title).RequiredText(200);
        RuleFor(x => x.Description).NotEmpty().WithMessage(ValidationMessages.Required);
        RuleFor(x => x.Location).RequiredText(200);
        RuleFor(x => x.Priority).NotEmpty().WithMessage(ValidationMessages.Required)
            .Must(v => Enum.TryParse<MaintenancePriority>(v, true, out _)).WithMessage("Geçersiz bakım önceliği.");
    }
}
