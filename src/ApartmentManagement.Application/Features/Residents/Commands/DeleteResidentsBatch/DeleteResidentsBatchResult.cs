namespace ApartmentManagement.Application.Features.Residents.Commands.DeleteResidentsBatch;

public record DeleteResidentsBatchResult(
    List<Guid> Deleted,
    List<Guid> NotFound
);
