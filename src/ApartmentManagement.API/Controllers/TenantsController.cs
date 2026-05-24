using ApartmentManagement.API.Extensions;
using ApartmentManagement.Application.Features.Tenants.Commands.CreateTenant;
using ApartmentManagement.Application.Features.Tenants.Commands.DeleteTenant;
using ApartmentManagement.Application.Features.Tenants.Commands.ToggleActiveStatus;
using ApartmentManagement.Application.Features.Tenants.Commands.UpdateTenant;
using ApartmentManagement.Application.Features.Tenants.Queries.GetTenantById;
using ApartmentManagement.Application.Features.Tenants.Queries.GetTenantStatistics;
using ApartmentManagement.Application.Features.Tenants.Queries.GetAllTenants;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

using ApartmentManagement.API.Controllers.Common;

namespace ApartmentManagement.API.Controllers;

[Route("api/v{version:apiVersion}/tenants")]
[Authorize(Roles = "SuperAdmin")]
public class TenantsController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] GetAllTenantsQuery query, CancellationToken ct)
        => (await Sender.Send(query, ct)).ToActionResult();

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
        => (await Sender.Send(new GetTenantByIdQuery(id), ct)).ToActionResult();

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTenantCommand cmd, CancellationToken ct)
        => (await Sender.Send(cmd, ct)).ToActionResult();

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTenantCommand cmd, CancellationToken ct)
        => (await Sender.Send(cmd with { Id = id }, ct)).ToActionResult();

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => (await Sender.Send(new DeleteTenantCommand(id), ct)).ToActionResult();

    [HttpPatch("{id:guid}/active-status")]
    public async Task<IActionResult> SetActiveStatus(Guid id, [FromBody] ToggleActiveStatusCommand cmd, CancellationToken ct)
        => (await Sender.Send(cmd with { Id = id }, ct)).ToActionResult();

    [HttpGet("{id:guid}/statistics")]
    public async Task<IActionResult> Statistics(Guid id, CancellationToken ct)
        => (await Sender.Send(new GetTenantStatisticsQuery(id), ct)).ToActionResult();
}
