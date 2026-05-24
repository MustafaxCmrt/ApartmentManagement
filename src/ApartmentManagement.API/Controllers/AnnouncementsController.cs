using ApartmentManagement.API.Extensions;
using ApartmentManagement.Application.Features.Announcements.Commands.CreateAnnouncement;
using ApartmentManagement.Application.Features.Announcements.Commands.DeleteAnnouncement;
using ApartmentManagement.Application.Features.Announcements.Commands.MarkAsRead;
using ApartmentManagement.Application.Features.Announcements.Commands.UpdateAnnouncement;
using ApartmentManagement.Application.Features.Announcements.Queries.GetAnnouncementById;
using ApartmentManagement.Application.Features.Announcements.Queries.GetAllAnnouncements;
using ApartmentManagement.Application.Features.Announcements.Queries.GetReadStatistics;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

using ApartmentManagement.API.Controllers.Common;

namespace ApartmentManagement.API.Controllers;

[Route("api/v{version:apiVersion}/announcements")]
public class AnnouncementsController : BaseController
{
    [HttpGet]
    [Authorize(Roles = "TenantAdmin,Resident")]
    public async Task<IActionResult> List([FromQuery] GetAllAnnouncementsQuery query, CancellationToken ct)
        => (await Sender.Send(query, ct)).ToActionResult();

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "TenantAdmin,Resident")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
        => (await Sender.Send(new GetAnnouncementByIdQuery(id), ct)).ToActionResult();

    [HttpPost]
    [Authorize(Roles = "TenantAdmin")]
    public async Task<IActionResult> Create([FromBody] CreateAnnouncementCommand cmd, CancellationToken ct)
        => (await Sender.Send(cmd, ct)).ToActionResult();

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "TenantAdmin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAnnouncementCommand cmd, CancellationToken ct)
        => (await Sender.Send(cmd with { Id = id }, ct)).ToActionResult();

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "TenantAdmin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => (await Sender.Send(new DeleteAnnouncementCommand(id), ct)).ToActionResult();

    [HttpPost("{id:guid}/read")]
    [Authorize(Roles = "Resident,TenantAdmin")]
    public async Task<IActionResult> MarkRead(Guid id, CancellationToken ct)
        => (await Sender.Send(new MarkAsReadCommand(id), ct)).ToActionResult();

    [HttpGet("{id:guid}/read-statistics")]
    [Authorize(Roles = "TenantAdmin")]
    public async Task<IActionResult> ReadStatistics(Guid id, CancellationToken ct)
        => (await Sender.Send(new GetReadStatisticsQuery(id), ct)).ToActionResult();
}
