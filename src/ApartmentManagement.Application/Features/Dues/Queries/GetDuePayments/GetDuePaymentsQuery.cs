using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Dues.Queries.GetDuePayments;

public record GetDuePaymentsQuery(Guid DueId) : IRequest<Result<List<DuePaymentDto>>>;
