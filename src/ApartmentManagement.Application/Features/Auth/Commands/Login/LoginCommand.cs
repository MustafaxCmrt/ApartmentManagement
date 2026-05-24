using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Auth.Commands.Login;

public record LoginCommand(string Email, string Sifre) : IRequest<Result<AuthResponseDto>>;
