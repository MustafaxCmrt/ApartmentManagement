using ApartmentManagement.API.Extensions;
using ApartmentManagement.Application.Features.Dues.Commands.CreateDue;
using ApartmentManagement.Application.Features.Dues.Commands.CreateBulkDues;
using ApartmentManagement.Application.Features.Dues.Commands.MarkPayment;
using ApartmentManagement.Application.Features.Dues.Commands.DeleteDue;
using ApartmentManagement.Application.Features.Dues.Commands.ReversePayment;
using ApartmentManagement.Application.Features.Dues.Commands.UpdateDue;
using ApartmentManagement.Application.Features.Dues.Queries.GetDueById;
using ApartmentManagement.Application.Features.Dues.Queries.GetAllDues;
using ApartmentManagement.Application.Features.Dues.Queries.GetDuePayments;
using ApartmentManagement.Application.Features.Dues.Queries.GetMonthlyDueReport;
using ApartmentManagement.Application.Features.Dues.Queries.GetMyDues;
using ApartmentManagement.Application.Features.Dues.Queries.GetOverdueDues;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

using ApartmentManagement.API.Controllers.Common;

namespace ApartmentManagement.API.Controllers;

[Route("api/v{version:apiVersion}/dues")]
public class DuesController : BaseController
{
    [HttpGet]
    [Authorize(Roles = "TenantAdmin")]
    public async Task<IActionResult> List([FromQuery] GetAllDuesQuery query, CancellationToken ct)
        => (await Sender.Send(query, ct)).ToActionResult();

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "TenantAdmin,Resident")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
        => (await Sender.Send(new GetDueByIdQuery(id), ct)).ToActionResult();

    [HttpPost]
    [Authorize(Roles = "TenantAdmin")]
    public async Task<IActionResult> Create([FromBody] CreateDueCommand cmd, CancellationToken ct)
        => (await Sender.Send(cmd, ct)).ToActionResult();

    [HttpPost("batch")]
    [Authorize(Roles = "TenantAdmin")]
    public async Task<IActionResult> CreateBatch([FromBody] CreateBulkDuesCommand cmd, CancellationToken ct)
        => (await Sender.Send(cmd, ct)).ToActionResult();

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "TenantAdmin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDueCommand cmd, CancellationToken ct)
        => (await Sender.Send(cmd with { Id = id }, ct)).ToActionResult();

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "TenantAdmin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => (await Sender.Send(new DeleteDueCommand(id), ct)).ToActionResult();

    [HttpPost("{id:guid}/payments")]
    [Authorize(Roles = "TenantAdmin")]
    public async Task<IActionResult> CreatePayment(Guid id, [FromBody] MarkPaymentCommand cmd, CancellationToken ct)
        => (await Sender.Send(cmd with { DueId = id }, ct)).ToActionResult();

    [HttpGet("{id:guid}/payments")]
    [Authorize(Roles = "TenantAdmin,Resident")]
    public async Task<IActionResult> Payments(Guid id, CancellationToken ct)
        => (await Sender.Send(new GetDuePaymentsQuery(id), ct)).ToActionResult();

    [HttpDelete("payments/{paymentId:guid}")]
    [Authorize(Roles = "TenantAdmin")]
    public async Task<IActionResult> DeletePayment(Guid paymentId, CancellationToken ct)
        => (await Sender.Send(new ReversePaymentCommand(paymentId), ct)).ToActionResult();

    [HttpGet("overdue")]
    [Authorize(Roles = "TenantAdmin")]
    public async Task<IActionResult> Overdue([FromQuery] GetOverdueDuesQuery query, CancellationToken ct)
        => (await Sender.Send(query, ct)).ToActionResult();

    [HttpGet("report")]
    [Authorize(Roles = "TenantAdmin")]
    public async Task<IActionResult> Report([FromQuery] GetMonthlyDueReportQuery query, CancellationToken ct)
        => (await Sender.Send(query, ct)).ToActionResult();

    [HttpGet("resident/me")]
    [Authorize(Roles = "Resident,TenantAdmin")]
    public async Task<IActionResult> ResidentMe([FromQuery] GetMyDuesQuery query, CancellationToken ct)
        => (await Sender.Send(query, ct)).ToActionResult();
}
