using ApartmentManagement.Application.Common.Models;
using MediatR;

namespace ApartmentManagement.Application.Features.MaintenanceTickets.Commands.UpdateMaintenanceTicket;

public record UpdateMaintenanceTicketCommand(
    Guid Id,
    string Title,
    string Description,
    string Location,
    string? AssignedTo,
    decimal? EstimatedCost,
    decimal? ActualCost
) : IRequest<Result>;
