using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.MaintenanceTickets.Commands.UpdatePriority;

public class UpdateMaintenancePriorityHandler : IRequestHandler<UpdateMaintenancePriorityCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public UpdateMaintenancePriorityHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateMaintenancePriorityCommand request, CancellationToken ct)
    {
        if (!Enum.TryParse<MaintenancePriority>(request.Priority, true, out var priority))
            return Result.Failure(Error.Validation("Invalid priority."));

        var ticket = await _db.MaintenanceTickets.FirstOrDefaultAsync(a => a.Id == request.Id, ct);
        if (ticket is null)
            return Result.Failure(Error.NotFound("Maintenance Ticket"));

        ticket.Priority = priority;
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
