using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Tenants.Commands.CreateTenant;

public record CreateTenantCommand(
    string Name,
    string ShortName,
    string ContactEmail,
    string? ContactPhone,
    string? Address,
    int MaxApartmentCount,
    DateTime? SubscriptionEnd,
    string? LogoUrl
) : IRequest<Result<TenantDto>>;
