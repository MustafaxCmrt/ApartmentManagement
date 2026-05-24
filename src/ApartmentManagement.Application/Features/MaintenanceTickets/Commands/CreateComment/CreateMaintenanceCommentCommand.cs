using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.MaintenanceTickets.Commands.CreateComment;

public record CreateMaintenanceCommentCommand(Guid MaintenanceTicketId, string Comment) : IRequest<Result<MaintenanceCommentDto>>;
