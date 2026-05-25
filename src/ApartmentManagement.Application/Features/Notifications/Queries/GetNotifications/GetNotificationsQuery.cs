using ApartmentManagement.Application.Common.DTOs;
using ApartmentManagement.Application.Common.Models;
using MediatR;

namespace ApartmentManagement.Application.Features.Notifications.Queries.GetNotifications;

public record GetNotificationsQuery(int PageNumber = 1, int PageSize = 20, bool? IsRead = null)
    : IRequest<Result<PagedResult<NotificationDto>>>;
