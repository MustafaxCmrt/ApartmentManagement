using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Meetings.Queries.GetAllMeetings;

public record GetAllMeetingsQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string? Status = null,
    int? Year = null
) : IRequest<Result<PagedResult<MeetingDto>>>;
