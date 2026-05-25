using ApartmentManagement.Application.Common.Models;
using MediatR;

namespace ApartmentManagement.Application.Features.Meetings.Commands.RemoveParticipant;

public record RemoveParticipantCommand(Guid MeetingId, Guid ParticipantId) : IRequest<Result>;
