using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.MaintenanceTickets.Queries.GetMaintenanceTicketById;

public record GetMaintenanceTicketByIdQuery(Guid Id) : IRequest<Result<MaintenanceTicketDetailDto>>;
