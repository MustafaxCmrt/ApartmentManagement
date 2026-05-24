using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.MaintenanceTickets.Commands.CreateMaintenanceTicket;

public record CreateMaintenanceTicketCommand(
    Guid? ApartmentId,
    string Title,
    string Description,
    string Location,
    string Priority
) : IRequest<Result<MaintenanceTicketDto>>;
