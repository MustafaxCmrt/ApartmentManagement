using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Residents.Commands.CreateInvite;

public record CreateInviteCommand(Guid ResidentId) : IRequest<Result<InviteResponseDto>>;
