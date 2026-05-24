using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Auth.Commands.RegisterAdmin;

public record RegisterAdminCommand(
    string ApartmanAdi,
    string ApartmanKisaAd,
    string ContactEmail,
    string? ContactPhone,
    string AdminAdSoyad,
    string AdminEmail,
    string AdminTelefon,
    string Sifre,
    string SifreTekrar
) : IRequest<Result<AuthResponseDto>>;
