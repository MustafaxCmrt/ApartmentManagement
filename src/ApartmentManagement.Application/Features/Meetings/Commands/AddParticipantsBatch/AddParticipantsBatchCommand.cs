using ApartmentManagement.Application.Common.DTOs;
using ApartmentManagement.Application.Common.Models;
using MediatR;

namespace ApartmentManagement.Application.Features.Meetings.Commands.AddParticipantsBatch;

public record AddParticipantsBatchCommand(
    Guid MeetingId,
    List<Guid> ApartmentIds
) : IRequest<Result<List<ParticipantDto>>>;
