using ApartmentManagement.API.Extensions;
using ApartmentManagement.Application.Features.Buildings.Commands.CreateBuilding;
using ApartmentManagement.Application.Features.Buildings.Commands.DeleteBuilding;
using ApartmentManagement.Application.Features.Buildings.Commands.DeleteBuildingsBatch;
using ApartmentManagement.Application.Features.Buildings.Commands.UpdateBuilding;
using ApartmentManagement.Application.Features.Buildings.Queries.GetBuildingById;
using ApartmentManagement.Application.Features.Buildings.Queries.GetBuildingApartments;
using ApartmentManagement.Application.Features.Buildings.Queries.GetAllBuildings;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

using ApartmentManagement.API.Controllers.Common;

namespace ApartmentManagement.API.Controllers;

[Route("api/v{version:apiVersion}/buildings")]
[Authorize(Roles = "TenantAdmin")]
public class BuildingsController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] GetAllBuildingsQuery query, CancellationToken ct)
        => (await Sender.Send(query, ct)).ToActionResult();

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
        => (await Sender.Send(new GetBuildingByIdQuery(id), ct)).ToActionResult();

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBuildingCommand cmd, CancellationToken ct)
        => (await Sender.Send(cmd, ct)).ToActionResult();

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBuildingCommand cmd, CancellationToken ct)
        => (await Sender.Send(cmd with { Id = id }, ct)).ToActionResult();

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => (await Sender.Send(new DeleteBuildingCommand(id), ct)).ToActionResult();

    [HttpDelete("batch")]
    public async Task<IActionResult> DeleteBatch([FromBody] DeleteBuildingsBatchCommand cmd, CancellationToken ct)
        => (await Sender.Send(cmd, ct)).ToActionResult();

    [HttpGet("{id:guid}/apartments")]
    public async Task<IActionResult> Apartments(Guid id, CancellationToken ct)
        => (await Sender.Send(new GetBuildingApartmentsQuery(id), ct)).ToActionResult();
}
