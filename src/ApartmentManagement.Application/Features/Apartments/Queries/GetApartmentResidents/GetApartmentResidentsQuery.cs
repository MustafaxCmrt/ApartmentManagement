using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Apartments.Queries.GetApartmentResidents;

public record GetApartmentResidentsQuery(Guid ApartmentId) : IRequest<Result<List<ResidentDto>>>;
