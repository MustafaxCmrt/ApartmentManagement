using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Residents.Queries.GetAllResidents;

public record GetAllResidentsQuery(
    int PageNumber = 1,
    int PageSize = 20,
    Guid? ApartmentId = null,
    string? ResidentType = null,
    bool? IsActive = null
) : IRequest<Result<PagedResult<ResidentDto>>>;
