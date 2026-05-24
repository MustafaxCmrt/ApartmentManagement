using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Dues.Commands.MarkPayment;

public record MarkPaymentCommand(
    Guid DueId,
    decimal PaidAmount,
    DateTime PaymentDate,
    string PaymentMethod,
    string? Description,
    string? ReceiptNumber
) : IRequest<Result<DuePaymentDto>>;
