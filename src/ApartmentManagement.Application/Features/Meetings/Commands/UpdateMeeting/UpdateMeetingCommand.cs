using ApartmentManagement.Application.Common.Models;
using MediatR;

namespace ApartmentManagement.Application.Features.Meetings.Commands.UpdateMeeting;

public record UpdateMeetingCommand(
    Guid Id,
    string Title,
    DateTime MeetingDate,
    string Venue,
    string Agenda,
    string Status
) : IRequest<Result>;
