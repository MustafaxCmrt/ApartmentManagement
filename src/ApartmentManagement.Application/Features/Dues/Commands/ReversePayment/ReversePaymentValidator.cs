using ApartmentManagement.Application.Common.Validation;
using FluentValidation;

namespace ApartmentManagement.Application.Features.Dues.Commands.ReversePayment;

public class ReversePaymentValidator : AbstractValidator<ReversePaymentCommand>
{
    public ReversePaymentValidator()
    {
        RuleFor(x => x.PaymentId).RequiredGuid();
    }
}
