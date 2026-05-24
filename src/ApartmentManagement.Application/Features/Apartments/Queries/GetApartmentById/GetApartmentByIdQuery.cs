using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Apartments.Queries.GetApartmentById;

public record GetApartmentByIdQuery(Guid Id) : IRequest<Result<ApartmentDetailDto>>;
