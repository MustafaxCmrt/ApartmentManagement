using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using ApartmentManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.MaintenanceTickets.Commands.CreateComment;

public class CreateMaintenanceCommentHandler : IRequestHandler<CreateMaintenanceCommentCommand, Result<MaintenanceCommentDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly ICurrentTenantService _currentTenant;

    public CreateMaintenanceCommentHandler(
        IApplicationDbContext db,
        ICurrentUserService currentUser,
        ICurrentTenantService currentTenant)
    {
        _db = db;
        _currentUser = currentUser;
        _currentTenant = currentTenant;
    }

    public async Task<Result<MaintenanceCommentDto>> Handle(CreateMaintenanceCommentCommand request, CancellationToken ct)
    {
        if (_currentTenant.TenantId is not { } tenantId || _currentUser.UserId is not { } userId)
            return Result<MaintenanceCommentDto>.Failure(Error.Unauthorized());

        var ticketExists = await _db.MaintenanceTickets.AnyAsync(a => a.Id == request.MaintenanceTicketId, ct);
        if (!ticketExists)
            return Result<MaintenanceCommentDto>.Failure(Error.NotFound("Maintenance Ticket"));

        var yorum = new MaintenanceComment
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            MaintenanceTicketId = request.MaintenanceTicketId,
            UserId = userId,
            Comment = request.Comment
        };

        _db.MaintenanceComments.Add(yorum);
        await _db.SaveChangesAsync(ct);

        var userFullName = await _db.Users
            .Where(u => u.Id == yorum.UserId)
            .Select(u => u.FullName)
            .FirstOrDefaultAsync(ct);

        return Result<MaintenanceCommentDto>.Success(new MaintenanceCommentDto
        {
            Id = yorum.Id,
            MaintenanceTicketId = yorum.MaintenanceTicketId,
            UserId = yorum.UserId,
            UserFullName = userFullName,
            Comment = yorum.Comment,
            CreatedAt = yorum.CreatedAt
        });
    }
}
