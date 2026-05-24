using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Meetings.Queries.GetMeetingById;

public record GetMeetingByIdQuery(Guid Id) : IRequest<Result<MeetingDetailDto>>;
