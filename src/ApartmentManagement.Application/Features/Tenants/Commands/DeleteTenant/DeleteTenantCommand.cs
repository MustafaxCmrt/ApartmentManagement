using ApartmentManagement.Application.Common.Models;
using MediatR;

namespace ApartmentManagement.Application.Features.Tenants.Commands.DeleteTenant;

public record DeleteTenantCommand(Guid Id) : IRequest<Result>;
