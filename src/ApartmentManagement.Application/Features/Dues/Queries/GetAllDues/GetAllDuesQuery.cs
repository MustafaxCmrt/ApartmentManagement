using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Dues.Queries.GetAllDues;

public record GetAllDuesQuery(
    int PageNumber = 1,
    int PageSize = 20,
    DateTime? Period = null,
    string? Status = null,
    Guid? BuildingId = null,
    Guid? ApartmentId = null
) : IRequest<Result<PagedResult<DueDto>>>;
