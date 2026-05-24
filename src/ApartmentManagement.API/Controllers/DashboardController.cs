using ApartmentManagement.API.Extensions;
using ApartmentManagement.Application.Features.Dashboard.Queries.GetDuesTrend;
using ApartmentManagement.Application.Features.Dashboard.Queries.GetSummary;
using ApartmentManagement.Application.Features.Dashboard.Queries.GetRecentActivities;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

using ApartmentManagement.API.Controllers.Common;

namespace ApartmentManagement.API.Controllers;

[Route("api/v{version:apiVersion}/dashboard")]
[Authorize(Roles = "TenantAdmin")]
public class DashboardController : BaseController
{
    [HttpGet("summary")]
    public async Task<IActionResult> Summary(CancellationToken ct)
        => (await Sender.Send(new GetSummaryQuery(), ct)).ToActionResult();

    [HttpGet("membership-fee-trend")]
    public async Task<IActionResult> DueTrend([FromQuery] GetDuesTrendQuery query, CancellationToken ct)
        => (await Sender.Send(query, ct)).ToActionResult();

    [HttpGet("recent-activities")]
    public async Task<IActionResult> RecentActivities([FromQuery] GetRecentActivitiesQuery query, CancellationToken ct)
        => (await Sender.Send(query, ct)).ToActionResult();
}
