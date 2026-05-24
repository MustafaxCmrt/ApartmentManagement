using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Apartments.Queries.GetAllApartments;

public record GetAllApartmentsQuery(
    int PageNumber = 1,
    int PageSize = 20,
    Guid? BuildingId = null,
    string? OccupancyStatus = null
) : IRequest<Result<PagedResult<ApartmentDto>>>;
