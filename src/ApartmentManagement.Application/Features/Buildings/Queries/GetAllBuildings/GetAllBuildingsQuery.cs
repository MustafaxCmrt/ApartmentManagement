using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Buildings.Queries.GetAllBuildings;

public record GetAllBuildingsQuery(int PageNumber = 1, int PageSize = 20, string? Search = null)
    : IRequest<Result<PagedResult<BuildingDto>>>;
