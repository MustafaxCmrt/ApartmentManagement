using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.MaintenanceTickets.Queries.GetComments;

public record GetMaintenanceCommentsQuery(Guid MaintenanceTicketId) : IRequest<Result<List<MaintenanceCommentDto>>>;
