using ApartmentManagement.Application.Common.Validation;
using ApartmentManagement.Domain.Enums;
using FluentValidation;

namespace ApartmentManagement.Application.Features.MaintenanceTickets.Commands.UpdateStatus;

public class UpdateMaintenanceStatusValidator : AbstractValidator<UpdateMaintenanceStatusCommand>
{
    public UpdateMaintenanceStatusValidator()
    {
        RuleFor(x => x.Id).RequiredGuid();
        RuleFor(x => x.Status).NotEmpty().WithMessage(ValidationMessages.Required)
            .Must(v => Enum.TryParse<MaintenanceStatus>(v, true, out _)).WithMessage("Geçersiz bakım durumu.");
    }
}
