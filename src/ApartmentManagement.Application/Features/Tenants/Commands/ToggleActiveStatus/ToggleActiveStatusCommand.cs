using ApartmentManagement.Application.Common.Models;
using MediatR;

namespace ApartmentManagement.Application.Features.Tenants.Commands.ToggleActiveStatus;

public record ToggleActiveStatusCommand(Guid Id, bool IsActive) : IRequest<Result>;
