using ApartmentManagement.API.Extensions;
using ApartmentManagement.Application.Features.Meetings.Commands.CreateMeeting;
using ApartmentManagement.Application.Features.Meetings.Commands.DeleteMeeting;
using ApartmentManagement.Application.Features.Meetings.Commands.UpdateMinutes;
using ApartmentManagement.Application.Features.Meetings.Commands.UpdateMeeting;
using ApartmentManagement.Application.Features.Meetings.Commands.AddParticipant;
using ApartmentManagement.Application.Features.Meetings.Commands.AddParticipantsBatch;
using ApartmentManagement.Application.Features.Meetings.Commands.RemoveParticipant;
using ApartmentManagement.Application.Features.Meetings.Commands.UpdateAttendanceStatus;
using ApartmentManagement.Application.Features.Meetings.Queries.GetMeetingById;
using ApartmentManagement.Application.Features.Meetings.Queries.GetAllMeetings;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

using ApartmentManagement.API.Controllers.Common;

namespace ApartmentManagement.API.Controllers;

[Route("api/v{version:apiVersion}/meetings")]
[Authorize(Roles = "TenantAdmin")]
public class MeetingsController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] GetAllMeetingsQuery query, CancellationToken ct)
        => (await Sender.Send(query, ct)).ToActionResult();

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
        => (await Sender.Send(new GetMeetingByIdQuery(id), ct)).ToActionResult();

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMeetingCommand cmd, CancellationToken ct)
        => (await Sender.Send(cmd, ct)).ToActionResult();

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMeetingCommand cmd, CancellationToken ct)
        => (await Sender.Send(cmd with { Id = id }, ct)).ToActionResult();

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => (await Sender.Send(new DeleteMeetingCommand(id), ct)).ToActionResult();

    [HttpGet("{id:guid}/participants")]
    public async Task<IActionResult> Participants(Guid id, CancellationToken ct)
        => (await Sender.Send(new GetMeetingByIdQuery(id), ct)).ToActionResult();

    [HttpPut("{id:guid}/participants/{participantId:guid}")]
    public async Task<IActionResult> UpdateParticipant(Guid id, Guid participantId, [FromBody] UpdateAttendanceStatusCommand cmd, CancellationToken ct)
        => (await Sender.Send(cmd with { ParticipantId = participantId }, ct)).ToActionResult();

    [HttpPost("{id:guid}/participants")]
    public async Task<IActionResult> AddParticipant(Guid id, [FromBody] AddParticipantCommand cmd, CancellationToken ct)
        => (await Sender.Send(cmd with { MeetingId = id }, ct)).ToActionResult();

    [HttpDelete("{id:guid}/participants/{participantId:guid}")]
    public async Task<IActionResult> RemoveParticipant(Guid id, Guid participantId, CancellationToken ct)
        => (await Sender.Send(new RemoveParticipantCommand(id, participantId), ct)).ToActionResult();

    [HttpPost("{id:guid}/participants/batch")]
    public async Task<IActionResult> AddParticipantsBatch(Guid id, [FromBody] AddParticipantsBatchCommand cmd, CancellationToken ct)
        => (await Sender.Send(cmd with { MeetingId = id }, ct)).ToActionResult();

    [HttpPut("{id:guid}/minutes")]
    public async Task<IActionResult> SaveMinutes(Guid id, [FromBody] UpdateMinutesCommand cmd, CancellationToken ct)
        => (await Sender.Send(cmd with { Id = id }, ct)).ToActionResult();
}
