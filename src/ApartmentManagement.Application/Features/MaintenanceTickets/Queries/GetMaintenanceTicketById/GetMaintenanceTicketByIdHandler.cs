using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.MaintenanceTickets.Queries.GetMaintenanceTicketById;

public class GetMaintenanceTicketByIdHandler : IRequestHandler<GetMaintenanceTicketByIdQuery, Result<MaintenanceTicketDetailDto>>
{
    private readonly IApplicationDbContext _db;

    public GetMaintenanceTicketByIdHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<MaintenanceTicketDetailDto>> Handle(GetMaintenanceTicketByIdQuery request, CancellationToken ct)
    {
        var dto = await _db.MaintenanceTickets.AsNoTracking()
            .Where(a => a.Id == request.Id)
            .Select(a => new MaintenanceTicketDetailDto
            {
                Id = a.Id,
                TenantId = a.TenantId,
                ApartmentId = a.ApartmentId,
                ApartmentNumber = a.Apartment != null ? a.Apartment.ApartmentNumber : null,
                RequestedByUserId = a.RequestedByUserId,
                RequestedByFullName = a.RequestedByUser != null ? a.RequestedByUser.FullName : null,
                Title = a.Title,
                Description = a.Description,
                Location = a.Location,
                Priority = a.Priority.ToString(),
                Status = a.Status.ToString(),
                AssignedTo = a.AssignedTo,
                CompletedAt = a.CompletedAt,
                EstimatedCost = a.EstimatedCost,
                ActualCost = a.ActualCost,
                CreatedAt = a.CreatedAt,
                CommentCount = a.Comments.Count()
            })
            .FirstOrDefaultAsync(ct);

        if (dto is null)
            return Result<MaintenanceTicketDetailDto>.Failure(Error.NotFound("Maintenance Ticket"));

        return Result<MaintenanceTicketDetailDto>.Success(dto);
    }
}
