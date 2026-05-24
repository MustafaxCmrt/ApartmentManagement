using ApartmentManagement.Application.Common.Models;
using MediatR;

namespace ApartmentManagement.Application.Features.Announcements.Commands.MarkAsRead;

public record MarkAsReadCommand(Guid AnnouncementId) : IRequest<Result>;
