using ApartmentManagement.Application.Common.Models;
using MediatR;

namespace ApartmentManagement.Application.Features.Dues.Commands.ReversePayment;

public record ReversePaymentCommand(Guid PaymentId) : IRequest<Result>;
