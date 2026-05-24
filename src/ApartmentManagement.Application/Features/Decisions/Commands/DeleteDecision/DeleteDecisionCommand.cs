using ApartmentManagement.Application.Common.Models;
using MediatR;

namespace ApartmentManagement.Application.Features.Decisions.Commands.DeleteDecision;

public record DeleteDecisionCommand(Guid Id) : IRequest<Result>;
