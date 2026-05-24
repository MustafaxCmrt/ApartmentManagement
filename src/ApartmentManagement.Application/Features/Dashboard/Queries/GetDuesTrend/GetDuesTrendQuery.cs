using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Dashboard.Queries.GetDuesTrend;

public record GetDuesTrendQuery(int MonthCount = 6) : IRequest<Result<DuesTrendDto>>;
