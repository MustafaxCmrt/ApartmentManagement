using ApartmentManagement.API.Extensions;
using ApartmentManagement.Application.Features.Auth.Commands.ChangePassword;
using ApartmentManagement.Application.Features.Auth.Commands.AcceptInvite;
using ApartmentManagement.Application.Features.Auth.Commands.ForgotPassword;
using ApartmentManagement.Application.Features.Auth.Commands.Login;
using ApartmentManagement.Application.Features.Auth.Commands.Logout;
using ApartmentManagement.Application.Features.Auth.Commands.RefreshToken;
using ApartmentManagement.Application.Features.Auth.Commands.RegisterAdmin;
using ApartmentManagement.Application.Features.Auth.Commands.ResetPassword;
using ApartmentManagement.Application.Features.Auth.Queries.Me;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

using ApartmentManagement.API.Controllers.Common;

namespace ApartmentManagement.API.Controllers;

[Route("api/v{version:apiVersion}/auth")]
public class AuthController : BaseController
{
    [HttpPost("register-manager")]
    [AllowAnonymous]
    [EnableRateLimiting("auth-strict")]
    public async Task<IActionResult> RegisterAdmin([FromBody] RegisterAdminCommand cmd, CancellationToken ct)
        => (await Sender.Send(cmd, ct)).ToActionResult();

    [HttpPost("accept-invite")]
    [AllowAnonymous]
    [EnableRateLimiting("auth-strict")]
    public async Task<IActionResult> AcceptInvite([FromBody] AcceptInviteCommand cmd, CancellationToken ct)
        => (await Sender.Send(cmd, ct)).ToActionResult();

    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting("auth-strict")]
    public async Task<IActionResult> Login([FromBody] LoginCommand cmd, CancellationToken ct)
        => (await Sender.Send(cmd, ct)).ToActionResult();

    [HttpPost("refresh")]
    [AllowAnonymous]
    [EnableRateLimiting("auth-strict")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand cmd, CancellationToken ct)
        => (await Sender.Send(cmd, ct)).ToActionResult();

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutCommand cmd, CancellationToken ct)
        => (await Sender.Send(cmd, ct)).ToActionResult();

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand cmd, CancellationToken ct)
        => (await Sender.Send(cmd, ct)).ToActionResult();

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [EnableRateLimiting("auth-strict")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand cmd, CancellationToken ct)
        => (await Sender.Send(cmd, ct)).ToActionResult();

    [HttpPost("reset-password")]
    [AllowAnonymous]
    [EnableRateLimiting("auth-strict")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand cmd, CancellationToken ct)
        => (await Sender.Send(cmd, ct)).ToActionResult();

    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken ct)
        => (await Sender.Send(new MeQuery(), ct)).ToActionResult();
}
