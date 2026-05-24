using ApartmentManagement.Application.Common.Validation;
using ApartmentManagement.Domain.Enums;
using FluentValidation;

namespace ApartmentManagement.Application.Features.Dues.Commands.MarkPayment;

public class MarkPaymentValidator : AbstractValidator<MarkPaymentCommand>
{
    public MarkPaymentValidator()
    {
        RuleFor(x => x.DueId).RequiredGuid();
        RuleFor(x => x.PaidAmount).GreaterThan(0).WithMessage(ValidationMessages.AmountPositive);
        RuleFor(x => x.PaymentDate).NotEmpty().WithMessage(ValidationMessages.Required);
        RuleFor(x => x.PaymentMethod).NotEmpty().WithMessage(ValidationMessages.Required)
            .Must(v => Enum.TryParse<PaymentMethod>(v, true, out _)).WithMessage("Geçersiz ödeme yöntemi.");
        RuleFor(x => x.Description).MaximumLength(500).WithMessage(ValidationMessages.MaxLength);
        RuleFor(x => x.ReceiptNumber).MaximumLength(50).WithMessage(ValidationMessages.MaxLength);
    }
}
