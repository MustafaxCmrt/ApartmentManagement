using ApartmentManagement.Application.Common.Models;
using MediatR;

namespace ApartmentManagement.Application.Features.Notifications.Commands.MarkAllAsRead;

public record MarkAllAsReadCommand() : IRequest<Result<MarkAllAsReadResultDto>>;
