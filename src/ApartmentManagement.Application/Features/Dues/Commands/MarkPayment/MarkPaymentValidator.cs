using FluentValidation;

namespace ApartmentManagement.Application.Features.Dues.Commands.MarkPayment;

public class MarkPaymentValidator : AbstractValidator<MarkPaymentCommand>
{
    public MarkPaymentValidator()
    {
        RuleFor(x => x.DueId).NotEmpty();
        RuleFor(x => x.PaidAmount).GreaterThan(0);
        RuleFor(x => x.PaymentDate).NotEmpty();
        RuleFor(x => x.PaymentMethod).NotEmpty();
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.ReceiptNumber).MaximumLength(50);
    }
}
