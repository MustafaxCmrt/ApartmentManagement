using ApartmentManagement.Application.Common.Models;
using MediatR;

namespace ApartmentManagement.Application.Features.Meetings.Commands.DeleteMeeting;

public record DeleteMeetingCommand(Guid Id) : IRequest<Result>;
