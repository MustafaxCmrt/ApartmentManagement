using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.MaintenanceTickets.Queries.GetAllMaintenanceTickets;

public record GetAllMaintenanceTicketsQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string? Status = null,
    string? Priority = null,
    Guid? ApartmentId = null
) : IRequest<Result<PagedResult<MaintenanceTicketDto>>>;
