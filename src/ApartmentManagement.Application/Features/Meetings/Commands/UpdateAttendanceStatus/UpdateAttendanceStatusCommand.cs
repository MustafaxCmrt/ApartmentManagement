using ApartmentManagement.Application.Common.Models;
using MediatR;

namespace ApartmentManagement.Application.Features.Meetings.Commands.UpdateAttendanceStatus;

public record UpdateAttendanceStatusCommand(
    Guid ParticipantId,
    string AttendanceStatus,
    Guid? ProxyApartmentId
) : IRequest<Result>;
