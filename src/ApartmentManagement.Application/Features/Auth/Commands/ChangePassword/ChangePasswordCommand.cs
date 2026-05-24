using ApartmentManagement.Application.Common.Models;
using MediatR;

namespace ApartmentManagement.Application.Features.Auth.Commands.ChangePassword;

public record ChangePasswordCommand(string EskiSifre, string YeniSifre, string YeniSifreTekrar) : IRequest<Result>;
