using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Meetings.Commands.AddParticipant;

public record AddParticipantCommand(
    Guid MeetingId,
    Guid ApartmentId,
    string AttendanceStatus,
    Guid? ProxyApartmentId
) : IRequest<Result<ParticipantDto>>;
