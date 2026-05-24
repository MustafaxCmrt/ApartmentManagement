using ApartmentManagement.Application.Common.Models;
using MediatR;

namespace ApartmentManagement.Application.Features.Residents.Commands.DeleteResident;

public record DeleteResidentCommand(Guid Id) : IRequest<Result>;
