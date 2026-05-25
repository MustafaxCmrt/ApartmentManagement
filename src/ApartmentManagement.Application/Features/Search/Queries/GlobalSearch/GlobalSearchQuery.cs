using ApartmentManagement.Application.Common.DTOs;
using ApartmentManagement.Application.Common.Models;
using MediatR;

namespace ApartmentManagement.Application.Features.Search.Queries.GlobalSearch;

public sealed record GlobalSearchQuery(string Q, int Limit = 8, string[]? Types = null)
    : IRequest<Result<GlobalSearchResultDto>>;
