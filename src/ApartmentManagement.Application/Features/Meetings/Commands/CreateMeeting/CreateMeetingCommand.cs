using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Meetings.Commands.CreateMeeting;

public record CreateMeetingCommand(
    string Title,
    DateTime MeetingDate,
    string Venue,
    string Agenda
) : IRequest<Result<MeetingDto>>;
