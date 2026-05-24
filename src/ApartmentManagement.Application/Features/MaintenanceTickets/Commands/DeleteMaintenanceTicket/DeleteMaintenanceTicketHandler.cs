using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.MaintenanceTickets.Commands.DeleteMaintenanceTicket;

public class DeleteMaintenanceTicketHandler : IRequestHandler<DeleteMaintenanceTicketCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public DeleteMaintenanceTicketHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteMaintenanceTicketCommand request, CancellationToken ct)
    {
        var ticket = await _db.MaintenanceTickets.FirstOrDefaultAsync(a => a.Id == request.Id, ct);
        if (ticket is null)
            return Result.Failure(Error.NotFound("Maintenance Ticket"));

        _db.MaintenanceTickets.Remove(ticket);
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
