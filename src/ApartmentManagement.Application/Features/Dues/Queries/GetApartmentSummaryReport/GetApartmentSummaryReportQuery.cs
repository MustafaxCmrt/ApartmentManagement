using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Dues.Queries.GetApartmentSummaryReport;

public record GetApartmentSummaryReportQuery(Guid? BuildingId = null) : IRequest<Result<List<ApartmentSummaryDto>>>;
