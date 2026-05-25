using ApartmentManagement.API.Controllers.Common;
using ApartmentManagement.API.Extensions;
using ApartmentManagement.Application.Features.Notifications.Commands.DeleteNotification;
using ApartmentManagement.Application.Features.Notifications.Commands.MarkAllAsRead;
using ApartmentManagement.Application.Features.Notifications.Commands.MarkAsRead;
using ApartmentManagement.Application.Features.Notifications.Queries.GetNotifications;
using ApartmentManagement.Application.Features.Notifications.Queries.GetUnreadCount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApartmentManagement.API.Controllers;

[Route("api/v{version:apiVersion}/notifications")]
[Authorize(Roles = "TenantAdmin,Resident")]
public class NotificationsController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] GetNotificationsQuery query, CancellationToken ct)
        => (await Sender.Send(query, ct)).ToActionResult();

    [HttpGet("unread-count")]
    public async Task<IActionResult> UnreadCount(CancellationToken ct)
        => (await Sender.Send(new GetUnreadCountQuery(), ct)).ToActionResult();

    [HttpPost("{id:guid}/read")]
    public async Task<IActionResult> MarkRead(Guid id, CancellationToken ct)
        => (await Sender.Send(new MarkAsReadCommand(id), ct)).ToActionResult();

    [HttpPost("mark-all-read")]
    public async Task<IActionResult> MarkAllRead(CancellationToken ct)
        => (await Sender.Send(new MarkAllAsReadCommand(), ct)).ToActionResult();

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => (await Sender.Send(new DeleteNotificationCommand(id), ct)).ToActionResult();
}
