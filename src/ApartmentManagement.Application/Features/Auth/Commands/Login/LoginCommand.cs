using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Auth.Commands.Login;

public record LoginCommand(string Telefon, string Sifre) : IRequest<Result<AuthResponseDto>>;
