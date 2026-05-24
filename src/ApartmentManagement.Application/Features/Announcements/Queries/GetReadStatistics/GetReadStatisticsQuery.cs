using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Announcements.Queries.GetReadStatistics;

public record GetReadStatisticsQuery(Guid AnnouncementId) : IRequest<Result<ReadStatisticsDto>>;
