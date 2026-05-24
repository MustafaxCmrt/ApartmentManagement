using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Apartments.Commands.CreateApartment;

public record CreateApartmentCommand(
    Guid BuildingId,
    string ApartmentNumber,
    int Floor,
    decimal? GrossSquareMeters,
    decimal? NetSquareMeters,
    string OccupancyStatus,
    decimal? DueMultiplier
) : IRequest<Result<ApartmentDto>>;
