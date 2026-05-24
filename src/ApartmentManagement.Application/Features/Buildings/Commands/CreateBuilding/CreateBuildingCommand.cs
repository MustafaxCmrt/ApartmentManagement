using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Buildings.Commands.CreateBuilding;

public record CreateBuildingCommand(
    string Name,
    string Address,
    int FloorCount,
    int ApartmentCount,
    int? ConstructionYear
) : IRequest<Result<BuildingDto>>;
