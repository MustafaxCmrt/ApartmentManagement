using ApartmentManagement.API.Extensions;
using ApartmentManagement.Application.Features.Apartments.Commands.CreateApartment;
using ApartmentManagement.Application.Features.Apartments.Commands.CreateApartmentsBatch;
using ApartmentManagement.Application.Features.Apartments.Commands.DeleteApartment;
using ApartmentManagement.Application.Features.Apartments.Commands.UpdateApartment;
using ApartmentManagement.Application.Features.Apartments.Queries.GetApartmentDues;
using ApartmentManagement.Application.Features.Apartments.Queries.GetApartmentById;
using ApartmentManagement.Application.Features.Apartments.Queries.GetAllApartments;
using ApartmentManagement.Application.Features.Apartments.Queries.GetApartmentResidents;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

using ApartmentManagement.API.Controllers.Common;

namespace ApartmentManagement.API.Controllers;

[Route("api/v{version:apiVersion}/apartments")]
[Authorize(Roles = "TenantAdmin")]
public class ApartmentsController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] GetAllApartmentsQuery query, CancellationToken ct)
        => (await Sender.Send(query, ct)).ToActionResult();

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
        => (await Sender.Send(new GetApartmentByIdQuery(id), ct)).ToActionResult();

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateApartmentCommand cmd, CancellationToken ct)
        => (await Sender.Send(cmd, ct)).ToActionResult();

    [HttpPost("batch")]
    public async Task<IActionResult> CreateBatch([FromBody] CreateApartmentsBatchCommand cmd, CancellationToken ct)
        => (await Sender.Send(cmd, ct)).ToActionResult();

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateApartmentCommand cmd, CancellationToken ct)
        => (await Sender.Send(cmd with { Id = id }, ct)).ToActionResult();

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => (await Sender.Send(new DeleteApartmentCommand(id), ct)).ToActionResult();

    [HttpGet("{id:guid}/residents")]
    public async Task<IActionResult> Residents(Guid id, CancellationToken ct)
        => (await Sender.Send(new GetApartmentResidentsQuery(id), ct)).ToActionResult();

    [HttpGet("{id:guid}/dues")]
    public async Task<IActionResult> Dues(Guid id, CancellationToken ct)
        => (await Sender.Send(new GetApartmentDuesQuery(id), ct)).ToActionResult();
}
