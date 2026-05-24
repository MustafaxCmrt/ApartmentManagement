using ApartmentManagement.Application.Common.Models;
using MediatR;

namespace ApartmentManagement.Application.Features.Residents.Commands.UpdateResident;

public record UpdateResidentCommand(
    Guid Id,
    string FullName,
    string Phone,
    string? Email,
    string ResidentType,
    DateTime MoveInDate,
    DateTime? MoveOutDate,
    bool IsPrimaryContact
) : IRequest<Result>;
