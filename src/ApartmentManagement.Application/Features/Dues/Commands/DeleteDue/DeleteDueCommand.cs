using ApartmentManagement.Application.Common.Models;
using MediatR;

namespace ApartmentManagement.Application.Features.Dues.Commands.DeleteDue;

public record DeleteDueCommand(Guid Id) : IRequest<Result>;
