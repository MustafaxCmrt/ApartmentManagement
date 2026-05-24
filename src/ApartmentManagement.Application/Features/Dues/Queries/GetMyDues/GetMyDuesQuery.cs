using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Dues.Queries.GetMyDues;

public record GetMyDuesQuery : IRequest<Result<List<DueDto>>>;
