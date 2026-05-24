using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.MaintenanceTickets.Queries.GetComments;

public class GetMaintenanceCommentsHandler : IRequestHandler<GetMaintenanceCommentsQuery, Result<List<MaintenanceCommentDto>>>
{
    private readonly IApplicationDbContext _db;

    public GetMaintenanceCommentsHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<List<MaintenanceCommentDto>>> Handle(GetMaintenanceCommentsQuery request, CancellationToken ct)
    {
        var exists = await _db.MaintenanceTickets.AnyAsync(a => a.Id == request.MaintenanceTicketId, ct);
        if (!exists)
            return Result<List<MaintenanceCommentDto>>.Failure(Error.NotFound("Maintenance Ticket"));

        var comments = await _db.MaintenanceComments.AsNoTracking()
            .Where(y => y.MaintenanceTicketId == request.MaintenanceTicketId)
            .OrderBy(y => y.CreatedAt)
            .Select(y => new MaintenanceCommentDto
            {
                Id = y.Id,
                MaintenanceTicketId = y.MaintenanceTicketId,
                UserId = y.UserId,
                Comment = y.Comment,
                CreatedAt = y.CreatedAt,
                UserFullName = _db.Users.Where(u => u.Id == y.UserId).Select(u => u.FullName).FirstOrDefault()
            })
            .ToListAsync(ct);

        return Result<List<MaintenanceCommentDto>>.Success(comments);
    }
}
