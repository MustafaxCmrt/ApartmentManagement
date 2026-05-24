using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Dashboard.Queries.GetRecentActivities;

public class GetRecentActivitiesHandler : IRequestHandler<GetRecentActivitiesQuery, Result<List<ActivityDto>>>
{
    private readonly IApplicationDbContext _db;

    public GetRecentActivitiesHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<List<ActivityDto>>> Handle(GetRecentActivitiesQuery request, CancellationToken ct)
    {
        var limit = Math.Clamp(request.Limit, 1, 50);

        var announcements = await _db.Announcements.AsNoTracking()
            .OrderByDescending(a => a.CreatedAt)
            .Take(limit)
            .Select(a => new ActivityDto
            {
                Type = "Announcement",
                Id = a.Id,
                Title = a.Title,
                Description = null,
                Date = a.CreatedAt
            })
            .ToListAsync(ct);

        var payments = await _db.DuePayments.AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .Take(limit)
            .Select(p => new ActivityDto
            {
                Type = "DuePayment",
                Id = p.Id,
                Title = "Due Payment",
                Description = $"Amount: {p.PaidAmount:F2} TL",
                Date = p.CreatedAt
            })
            .ToListAsync(ct);

        var maintenanceTickets = await _db.MaintenanceTickets.AsNoTracking()
            .OrderByDescending(t => t.CreatedAt)
            .Take(limit)
            .Select(t => new ActivityDto
            {
                Type = "MaintenanceTicket",
                Id = t.Id,
                Title = t.Title,
                Description = t.Location,
                Date = t.CreatedAt
            })
            .ToListAsync(ct);

        var activities = announcements
            .Concat(payments)
            .Concat(maintenanceTickets)
            .OrderByDescending(a => a.Date)
            .Take(limit)
            .ToList();

        return Result<List<ActivityDto>>.Success(activities);
    }
}
