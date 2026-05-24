using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.MaintenanceTickets.Commands.UpdateMaintenanceTicket;

public class UpdateMaintenanceTicketHandler : IRequestHandler<UpdateMaintenanceTicketCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public UpdateMaintenanceTicketHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateMaintenanceTicketCommand request, CancellationToken ct)
    {
        var ticket = await _db.MaintenanceTickets.FirstOrDefaultAsync(a => a.Id == request.Id, ct);
        if (ticket is null)
            return Result.Failure(Error.NotFound("Maintenance Ticket"));

        ticket.Title = request.Title.Trim();
        ticket.Description = request.Description;
        ticket.Location = request.Location.Trim();
        ticket.AssignedTo = request.AssignedTo;
        ticket.EstimatedCost = request.EstimatedCost;
        ticket.ActualCost = request.ActualCost;

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
