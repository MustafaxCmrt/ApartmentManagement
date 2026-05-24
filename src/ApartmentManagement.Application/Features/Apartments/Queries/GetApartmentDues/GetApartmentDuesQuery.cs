using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Apartments.Queries.GetApartmentDues;

public record GetApartmentDuesQuery(Guid ApartmentId) : IRequest<Result<List<DueDto>>>;
