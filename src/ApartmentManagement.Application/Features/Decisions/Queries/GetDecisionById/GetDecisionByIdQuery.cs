using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Decisions.Queries.GetDecisionById;

public record GetDecisionByIdQuery(Guid Id) : IRequest<Result<DecisionDto>>;
