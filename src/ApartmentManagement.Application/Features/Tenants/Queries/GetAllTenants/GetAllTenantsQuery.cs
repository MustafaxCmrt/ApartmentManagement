using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Tenants.Queries.GetAllTenants;

public record GetAllTenantsQuery(int PageNumber = 1, int PageSize = 20, string? Search = null, bool? IsActive = null)
    : IRequest<Result<PagedResult<TenantDto>>>;
