using ApartmentManagement.API.Extensions;
using ApartmentManagement.Application.Features.MaintenanceTickets.Commands.CreateMaintenanceTicket;
using ApartmentManagement.Application.Features.MaintenanceTickets.Commands.CreateComment;
using ApartmentManagement.Application.Features.MaintenanceTickets.Commands.DeleteMaintenanceTicket;
using ApartmentManagement.Application.Features.MaintenanceTickets.Commands.UpdateStatus;
using ApartmentManagement.Application.Features.MaintenanceTickets.Commands.UpdatePriority;
using ApartmentManagement.Application.Features.MaintenanceTickets.Commands.UpdateMaintenanceTicket;
using ApartmentManagement.Application.Features.MaintenanceTickets.Queries.GetMaintenanceTicketById;
using ApartmentManagement.Application.Features.MaintenanceTickets.Queries.GetAllMaintenanceTickets;
using ApartmentManagement.Application.Features.MaintenanceTickets.Queries.GetComments;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

using ApartmentManagement.API.Controllers.Common;

namespace ApartmentManagement.API.Controllers;

[Route("api/v{version:apiVersion}/maintenance-tickets")]
public class MaintenanceTicketsController : BaseController
{
    [HttpGet]
    [Authorize(Roles = "TenantAdmin,Resident")]
    public async Task<IActionResult> List([FromQuery] GetAllMaintenanceTicketsQuery query, CancellationToken ct)
        => (await Sender.Send(query, ct)).ToActionResult();

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "TenantAdmin,Resident")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
        => (await Sender.Send(new GetMaintenanceTicketByIdQuery(id), ct)).ToActionResult();

    [HttpPost]
    [Authorize(Roles = "TenantAdmin,Resident")]
    public async Task<IActionResult> Create([FromBody] CreateMaintenanceTicketCommand cmd, CancellationToken ct)
        => (await Sender.Send(cmd, ct)).ToActionResult();

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "TenantAdmin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMaintenanceTicketCommand cmd, CancellationToken ct)
        => (await Sender.Send(cmd with { Id = id }, ct)).ToActionResult();

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "TenantAdmin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => (await Sender.Send(new DeleteMaintenanceTicketCommand(id), ct)).ToActionResult();

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "TenantAdmin")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateMaintenanceStatusCommand cmd, CancellationToken ct)
        => (await Sender.Send(cmd with { Id = id }, ct)).ToActionResult();

    [HttpPatch("{id:guid}/priority")]
    [Authorize(Roles = "TenantAdmin")]
    public async Task<IActionResult> UpdatePriority(Guid id, [FromBody] UpdateMaintenancePriorityCommand cmd, CancellationToken ct)
        => (await Sender.Send(cmd with { Id = id }, ct)).ToActionResult();

    [HttpPost("{id:guid}/comment")]
    [Authorize(Roles = "TenantAdmin,Resident")]
    public async Task<IActionResult> CreateComment(Guid id, [FromBody] CreateMaintenanceCommentCommand cmd, CancellationToken ct)
        => (await Sender.Send(cmd with { MaintenanceTicketId = id }, ct)).ToActionResult();

    [HttpGet("{id:guid}/comments")]
    [Authorize(Roles = "TenantAdmin,Resident")]
    public async Task<IActionResult> Comments(Guid id, CancellationToken ct)
        => (await Sender.Send(new GetMaintenanceCommentsQuery(id), ct)).ToActionResult();
}
