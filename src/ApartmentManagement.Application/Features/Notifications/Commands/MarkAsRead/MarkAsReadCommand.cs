using ApartmentManagement.Application.Common.Models;
using MediatR;

namespace ApartmentManagement.Application.Features.Notifications.Commands.MarkAsRead;

public record MarkAsReadCommand(Guid Id) : IRequest<Result>;
