using ApartmentManagement.Application.Common.Models;
using MediatR;

namespace ApartmentManagement.Application.Features.Dues.Commands.CreateBulkDues;

public record CreateBulkDuesCommand(
    DateTime Period,
    decimal BaseAmount,
    DateTime DueDate,
    string? Description,
    Guid? BuildingId = null
) : IRequest<Result<int>>;
