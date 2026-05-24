using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Residents.Commands.CreateResident;

public record CreateResidentCommand(
    Guid ApartmentId,
    string FullName,
    string Phone,
    string? Email,
    string ResidentType,
    DateTime MoveInDate,
    bool IsPrimaryContact
) : IRequest<Result<ResidentDto>>;
