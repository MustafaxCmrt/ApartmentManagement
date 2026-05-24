using ApartmentManagement.Application.Common.Validation;
using ApartmentManagement.Domain.Enums;
using FluentValidation;

namespace ApartmentManagement.Application.Features.MaintenanceTickets.Commands.UpdatePriority;

public class UpdateMaintenancePriorityValidator : AbstractValidator<UpdateMaintenancePriorityCommand>
{
    public UpdateMaintenancePriorityValidator()
    {
        RuleFor(x => x.Id).RequiredGuid();
        RuleFor(x => x.Priority).NotEmpty().WithMessage(ValidationMessages.Required)
            .Must(v => Enum.TryParse<MaintenancePriority>(v, true, out _)).WithMessage("Geçersiz bakım önceliği.");
    }
}
