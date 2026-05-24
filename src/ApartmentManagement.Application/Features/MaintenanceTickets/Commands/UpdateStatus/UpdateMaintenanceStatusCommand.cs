using ApartmentManagement.Application.Common.Models;
using MediatR;

namespace ApartmentManagement.Application.Features.MaintenanceTickets.Commands.UpdateStatus;

public record UpdateMaintenanceStatusCommand(Guid Id, string Status) : IRequest<Result>;
