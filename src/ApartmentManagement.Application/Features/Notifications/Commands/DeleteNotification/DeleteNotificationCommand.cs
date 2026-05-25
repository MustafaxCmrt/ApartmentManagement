using ApartmentManagement.Application.Common.Models;
using MediatR;

namespace ApartmentManagement.Application.Features.Notifications.Commands.DeleteNotification;

public record DeleteNotificationCommand(Guid Id) : IRequest<Result>;
