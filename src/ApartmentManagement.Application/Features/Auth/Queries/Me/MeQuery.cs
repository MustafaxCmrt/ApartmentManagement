using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Auth.Queries.Me;

public record MeQuery : IRequest<Result<UserDto>>;
