using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Dues.Queries.GetOverdueDues;

public record GetOverdueDuesQuery(int PageNumber = 1, int PageSize = 20) : IRequest<Result<PagedResult<OverdueDueDto>>>;
