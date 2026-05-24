using ApartmentManagement.Application.Common.Models;
using MediatR;

namespace ApartmentManagement.Application.Features.MaintenanceTickets.Commands.UpdatePriority;

public record UpdateMaintenancePriorityCommand(Guid Id, string Priority) : IRequest<Result>;
