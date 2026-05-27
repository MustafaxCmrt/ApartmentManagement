namespace ApartmentManagement.Application.Features.Buildings.Commands.DeleteBuildingsBatch;

public record DeleteBuildingsBatchResult(
    List<Guid> Deleted,
    List<Guid> NotFound,
    List<Guid> Skipped
);
