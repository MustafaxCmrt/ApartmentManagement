using ApartmentManagement.Application.Common.Models;
using MediatR;

namespace ApartmentManagement.Application.Features.Residents.Commands.DeleteResidentsBatch;

public record DeleteResidentsBatchCommand(List<Guid> Ids) : IRequest<Result<DeleteResidentsBatchResult>>;
