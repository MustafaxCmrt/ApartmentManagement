using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Dashboard.Queries.GetSummary;

public record GetSummaryQuery : IRequest<Result<DashboardSummaryDto>>;
