using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Auth.Commands.AcceptInvite;

public record AcceptInviteCommand(string InviteToken, string Sifre, string SifreTekrar) : IRequest<Result<AuthResponseDto>>;
