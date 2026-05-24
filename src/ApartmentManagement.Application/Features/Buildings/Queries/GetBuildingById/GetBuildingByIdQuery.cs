using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Buildings.Queries.GetBuildingById;

public record GetBuildingByIdQuery(Guid Id) : IRequest<Result<BuildingDetailDto>>;
