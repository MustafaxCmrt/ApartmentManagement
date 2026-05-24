using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Dues.Commands.CreateDue;

public record CreateDueCommand(
    Guid ApartmentId,
    DateTime Period,
    decimal Amount,
    DateTime DueDate,
    string? Description
) : IRequest<Result<DueDto>>;
