using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using ApartmentManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.MaintenanceTickets.Queries.GetAllMaintenanceTickets;

public class GetAllMaintenanceTicketsHandler : IRequestHandler<GetAllMaintenanceTicketsQuery, Result<PagedResult<MaintenanceTicketDto>>>
{
    private readonly IApplicationDbContext _db;

    public GetAllMaintenanceTicketsHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<PagedResult<MaintenanceTicketDto>>> Handle(GetAllMaintenanceTicketsQuery request, CancellationToken ct)
    {
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var pageNumber = Math.Max(1, request.PageNumber);

        var query = _db.MaintenanceTickets.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<MaintenanceStatus>(request.Status, true, out var status))
            query = query.Where(a => a.Status == status);

        if (!string.IsNullOrWhiteSpace(request.Priority) &&
            Enum.TryParse<MaintenancePriority>(request.Priority, true, out var priority))
            query = query.Where(a => a.Priority == priority);

        if (request.ApartmentId.HasValue)
            query = query.Where(a => a.ApartmentId == request.ApartmentId.Value);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new MaintenanceTicketDto
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
                CreatedAt = a.CreatedAt
            })
            .ToListAsync(ct);

        return Result<PagedResult<MaintenanceTicketDto>>.Success(new PagedResult<MaintenanceTicketDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = total
        });
    }
}
