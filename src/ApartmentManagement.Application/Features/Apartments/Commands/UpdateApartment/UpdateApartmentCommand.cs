using ApartmentManagement.Application.Common.Models;
using MediatR;

namespace ApartmentManagement.Application.Features.Apartments.Commands.UpdateApartment;

public record UpdateApartmentCommand(
    Guid Id,
    string ApartmentNumber,
    int Floor,
    decimal? GrossSquareMeters,
    decimal? NetSquareMeters,
    string OccupancyStatus,
    decimal DueMultiplier
) : IRequest<Result>;
