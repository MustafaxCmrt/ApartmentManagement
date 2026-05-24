using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.MaintenanceTickets.Commands.UpdateStatus;

public class UpdateMaintenanceStatusHandler : IRequestHandler<UpdateMaintenanceStatusCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly IDateTimeProvider _dateTime;

    public UpdateMaintenanceStatusHandler(IApplicationDbContext db, IDateTimeProvider dateTime)
    {
        _db = db;
        _dateTime = dateTime;
    }

    public async Task<Result> Handle(UpdateMaintenanceStatusCommand request, CancellationToken ct)
    {
        if (!Enum.TryParse<MaintenanceStatus>(request.Status, true, out var status))
            return Result.Failure(Error.Validation("Invalid status."));

        var ticket = await _db.MaintenanceTickets.FirstOrDefaultAsync(a => a.Id == request.Id, ct);
        if (ticket is null)
            return Result.Failure(Error.NotFound("Maintenance Ticket"));

        ticket.Status = status;
        if (status == MaintenanceStatus.Completed && ticket.CompletedAt is null)
            ticket.CompletedAt = _dateTime.UtcNow;
        else if (status != MaintenanceStatus.Completed)
            ticket.CompletedAt = null;

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
