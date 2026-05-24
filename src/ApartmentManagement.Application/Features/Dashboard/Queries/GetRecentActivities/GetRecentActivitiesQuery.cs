using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Dashboard.Queries.GetRecentActivities;

public record GetRecentActivitiesQuery(int Limit = 10) : IRequest<Result<List<ActivityDto>>>;
