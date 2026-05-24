using ApartmentManagement.Application.Common.Models;
using MediatR;

namespace ApartmentManagement.Application.Features.Buildings.Commands.UpdateBuilding;

public record UpdateBuildingCommand(
    Guid Id,
    string Name,
    string Address,
    int FloorCount,
    int ApartmentCount,
    int? ConstructionYear
) : IRequest<Result>;
