using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using ApartmentManagement.Domain.Entities;
using ApartmentManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.MaintenanceTickets.Commands.CreateMaintenanceTicket;

public class CreateMaintenanceTicketHandler : IRequestHandler<CreateMaintenanceTicketCommand, Result<MaintenanceTicketDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly ICurrentTenantService _currentTenant;

    public CreateMaintenanceTicketHandler(
        IApplicationDbContext db,
        ICurrentUserService currentUser,
        ICurrentTenantService currentTenant)
    {
        _db = db;
        _currentUser = currentUser;
        _currentTenant = currentTenant;
    }

    public async Task<Result<MaintenanceTicketDto>> Handle(CreateMaintenanceTicketCommand request, CancellationToken ct)
    {
        if (_currentTenant.TenantId is not { } tenantId || _currentUser.UserId is not { } userId)
            return Result<MaintenanceTicketDto>.Failure(Error.Unauthorized());

        if (!Enum.TryParse<MaintenancePriority>(request.Priority, true, out var priority))
            return Result<MaintenanceTicketDto>.Failure(Error.Validation("Invalid priority."));

        if (request.ApartmentId.HasValue)
        {
            var apartmentExists = await _db.Apartments.AnyAsync(d => d.Id == request.ApartmentId.Value, ct);
            if (!apartmentExists)
                return Result<MaintenanceTicketDto>.Failure(Error.NotFound("Apartment"));
        }

        var ticket = new MaintenanceTicket
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ApartmentId = request.ApartmentId,
            RequestedByUserId = userId,
            Title = request.Title.Trim(),
            Description = request.Description,
            Location = request.Location.Trim(),
            Priority = priority,
            Status = MaintenanceStatus.Open
        };

        _db.MaintenanceTickets.Add(ticket);
        await _db.SaveChangesAsync(ct);

        return Result<MaintenanceTicketDto>.Success(new MaintenanceTicketDto
        {
            Id = ticket.Id,
            TenantId = ticket.TenantId,
            ApartmentId = ticket.ApartmentId,
            RequestedByUserId = ticket.RequestedByUserId,
            Title = ticket.Title,
            Description = ticket.Description,
            Location = ticket.Location,
            Priority = ticket.Priority.ToString(),
            Status = ticket.Status.ToString(),
            CreatedAt = ticket.CreatedAt
        });
    }
}
