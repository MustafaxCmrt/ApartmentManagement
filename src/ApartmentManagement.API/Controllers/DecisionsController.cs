using ApartmentManagement.API.Extensions;
using ApartmentManagement.Application.Features.Decisions.Commands.CreateDecision;
using ApartmentManagement.Application.Features.Decisions.Commands.DeleteDecision;
using ApartmentManagement.Application.Features.Decisions.Commands.UpdateDecision;
using ApartmentManagement.Application.Features.Decisions.Queries.GetDecisionById;
using ApartmentManagement.Application.Features.Decisions.Queries.GetDecisionByNumber;
using ApartmentManagement.Application.Features.Decisions.Queries.GetAllDecisions;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

using ApartmentManagement.API.Controllers.Common;

namespace ApartmentManagement.API.Controllers;

[Route("api/v{version:apiVersion}/decisions")]
[Authorize(Roles = "TenantAdmin")]
public class DecisionsController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] GetAllDecisionsQuery query, CancellationToken ct)
        => (await Sender.Send(query, ct)).ToActionResult();

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
        => (await Sender.Send(new GetDecisionByIdQuery(id), ct)).ToActionResult();

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDecisionCommand cmd, CancellationToken ct)
        => (await Sender.Send(cmd, ct)).ToActionResult();

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDecisionCommand cmd, CancellationToken ct)
        => (await Sender.Send(cmd with { Id = id }, ct)).ToActionResult();

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => (await Sender.Send(new DeleteDecisionCommand(id), ct)).ToActionResult();

    [HttpGet("decision-number/{decisionNumber:int}")]
    public async Task<IActionResult> GetDecisionByNumber(int decisionNumber, CancellationToken ct)
        => (await Sender.Send(new GetDecisionByNumberQuery(decisionNumber), ct)).ToActionResult();
}
