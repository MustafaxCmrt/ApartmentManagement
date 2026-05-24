using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Apartments.Commands.CreateApartmentsBatch;

public record ApartmentCreateItemInput(
    string ApartmentNumber,
    int Floor,
    decimal? GrossSquareMeters,
    decimal? NetSquareMeters,
    string OccupancyStatus,
    decimal? DueMultiplier
);

public record CreateApartmentsBatchCommand(
    Guid BuildingId,
    List<ApartmentCreateItemInput> Apartments
) : IRequest<Result<List<ApartmentDto>>>;
