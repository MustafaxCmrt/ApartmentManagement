using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Dues.Queries.GetDueById;

public record GetDueByIdQuery(Guid Id) : IRequest<Result<DueDetailDto>>;
