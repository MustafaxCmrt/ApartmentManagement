using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Buildings.Queries.GetBuildingApartments;

public record GetBuildingApartmentsQuery(Guid BuildingId) : IRequest<Result<List<ApartmentDto>>>;
