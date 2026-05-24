using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Dues.Queries.GetApartmentDueHistory;

public record GetApartmentDueHistoryQuery(Guid ApartmentId) : IRequest<Result<List<DueDto>>>;
