using ApartmentManagement.Application.Common.Models;
using MediatR;

namespace ApartmentManagement.Application.Features.Notifications.Queries.GetUnreadCount;

public record GetUnreadCountQuery() : IRequest<Result<UnreadCountDto>>;
