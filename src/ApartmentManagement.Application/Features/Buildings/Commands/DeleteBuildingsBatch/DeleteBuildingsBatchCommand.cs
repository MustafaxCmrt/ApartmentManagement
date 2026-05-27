using ApartmentManagement.Application.Common.Models;
using MediatR;

namespace ApartmentManagement.Application.Features.Buildings.Commands.DeleteBuildingsBatch;

public record DeleteBuildingsBatchCommand(List<Guid> Ids) : IRequest<Result<DeleteBuildingsBatchResult>>;
