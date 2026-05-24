using ApartmentManagement.Application.Common.Models;
using MediatR;

namespace ApartmentManagement.Application.Features.Tenants.Commands.UpdateTenant;

public record UpdateTenantCommand(
    Guid Id,
    string Name,
    string ContactEmail,
    string? ContactPhone,
    string? Address,
    int MaxApartmentCount,
    DateTime? SubscriptionEnd,
    string? LogoUrl
) : IRequest<Result>;
