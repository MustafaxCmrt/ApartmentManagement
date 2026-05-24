using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Decisions.Queries.GetAllDecisions;

public record GetAllDecisionsQuery(
    int PageNumber = 1,
    int PageSize = 20,
    int? Year = null,
    Guid? MeetingId = null
) : IRequest<Result<PagedResult<DecisionDto>>>;
