using ApartmentManagement.Application.Common.Models;
using MediatR;

namespace ApartmentManagement.Application.Features.Dues.Commands.UpdateDue;

public record UpdateDueCommand(
    Guid Id,
    decimal Amount,
    DateTime DueDate,
    string? Description
) : IRequest<Result>;
