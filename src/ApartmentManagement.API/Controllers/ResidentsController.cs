using ApartmentManagement.API.Extensions;
using ApartmentManagement.Application.Features.Residents.Commands.CreateResident;
using ApartmentManagement.Application.Features.Residents.Commands.DeleteResident;
using ApartmentManagement.Application.Features.Residents.Commands.CreateInvite;
using ApartmentManagement.Application.Features.Residents.Commands.UpdateResident;
using ApartmentManagement.Application.Features.Residents.Queries.GetResidentById;
using ApartmentManagement.Application.Features.Residents.Queries.GetAllResidents;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

using ApartmentManagement.API.Controllers.Common;

namespace ApartmentManagement.API.Controllers;

[Route("api/v{version:apiVersion}/residents")]
[Authorize(Roles = "TenantAdmin")]
public class ResidentsController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] GetAllResidentsQuery query, CancellationToken ct)
        => (await Sender.Send(query, ct)).ToActionResult();

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
        => (await Sender.Send(new GetResidentByIdQuery(id), ct)).ToActionResult();

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateResidentCommand cmd, CancellationToken ct)
        => (await Sender.Send(cmd, ct)).ToActionResult();

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateResidentCommand cmd, CancellationToken ct)
        => (await Sender.Send(cmd with { Id = id }, ct)).ToActionResult();

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => (await Sender.Send(new DeleteResidentCommand(id), ct)).ToActionResult();

    [HttpPost("{id:guid}/user-invite")]
    public async Task<IActionResult> CreateUserInvite(Guid id, [FromBody] CreateInviteCommand cmd, CancellationToken ct)
        => (await Sender.Send(cmd with { ResidentId = id }, ct)).ToActionResult();
}
