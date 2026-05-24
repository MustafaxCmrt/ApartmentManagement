using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Announcements.Queries.GetReadStatistics;

public class GetReadStatisticsHandler : IRequestHandler<GetReadStatisticsQuery, Result<ReadStatisticsDto>>
{
    private readonly IApplicationDbContext _db;

    public GetReadStatisticsHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<ReadStatisticsDto>> Handle(GetReadStatisticsQuery request, CancellationToken ct)
    {
        var announcementExists = await _db.Announcements.AnyAsync(d => d.Id == request.AnnouncementId, ct);
        if (!announcementExists)
            return Result<ReadStatisticsDto>.Failure(Error.NotFound("Announcement"));

        var reads = await _db.AnnouncementReads.AsNoTracking()
            .Where(o => o.AnnouncementId == request.AnnouncementId)
            .ToListAsync(ct);

        var dto = new ReadStatisticsDto
        {
            AnnouncementId = request.AnnouncementId,
            TotalReads = reads.Count,
            UniqueUserCount = reads.Select(o => o.UserId).Distinct().Count(),
            FirstReadAt = reads.Count > 0 ? reads.Min(o => o.ReadAt) : (DateTime?)null,
            LastReadAt = reads.Count > 0 ? reads.Max(o => o.ReadAt) : (DateTime?)null
        };

        return Result<ReadStatisticsDto>.Success(dto);
    }
}
