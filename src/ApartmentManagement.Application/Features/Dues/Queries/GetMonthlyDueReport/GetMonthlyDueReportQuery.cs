using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Dues.Queries.GetMonthlyDueReport;

public record GetMonthlyDueReportQuery(int Year, int Month) : IRequest<Result<MonthlyReportDto>>;
