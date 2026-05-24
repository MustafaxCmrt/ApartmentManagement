using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Decisions.Queries.GetDecisionByNumber;

public record GetDecisionByNumberQuery(int DecisionNumber) : IRequest<Result<DecisionDto>>;
