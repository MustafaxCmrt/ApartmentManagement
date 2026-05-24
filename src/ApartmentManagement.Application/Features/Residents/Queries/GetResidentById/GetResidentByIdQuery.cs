using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Residents.Queries.GetResidentById;

public record GetResidentByIdQuery(Guid Id) : IRequest<Result<ResidentDetailDto>>;
