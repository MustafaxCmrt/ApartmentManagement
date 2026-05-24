using ApartmentManagement.Application.Common.Models;
using MediatR;

namespace ApartmentManagement.Application.Features.Announcements.Commands.DeleteAnnouncement;

public record DeleteAnnouncementCommand(Guid Id) : IRequest<Result>;
