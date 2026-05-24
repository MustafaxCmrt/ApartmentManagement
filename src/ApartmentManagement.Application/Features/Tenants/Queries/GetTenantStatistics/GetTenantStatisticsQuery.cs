using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Tenants.Queries.GetTenantStatistics;

public record GetTenantStatisticsQuery(Guid TenantId) : IRequest<Result<TenantStatisticsDto>>;
