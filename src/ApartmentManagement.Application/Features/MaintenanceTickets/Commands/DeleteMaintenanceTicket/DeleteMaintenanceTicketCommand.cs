using ApartmentManagement.Application.Common.Models;
using MediatR;

namespace ApartmentManagement.Application.Features.MaintenanceTickets.Commands.DeleteMaintenanceTicket;

public record DeleteMaintenanceTicketCommand(Guid Id) : IRequest<Result>;
