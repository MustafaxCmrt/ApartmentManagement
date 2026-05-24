using ApartmentManagement.Application.Common.Models;
using MediatR;

namespace ApartmentManagement.Application.Features.Apartments.Commands.DeleteApartment;

public record DeleteApartmentCommand(Guid Id) : IRequest<Result>;
