using ApartmentManagement.Application.Common.Models;
using MediatR;

namespace ApartmentManagement.Application.Features.Buildings.Commands.DeleteBuilding;

public record DeleteBuildingCommand(Guid Id) : IRequest<Result>;
