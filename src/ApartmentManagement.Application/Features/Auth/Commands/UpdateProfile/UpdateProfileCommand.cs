using ApartmentManagement.Application.Common.DTOs;
using ApartmentManagement.Application.Common.Models;
using MediatR;

namespace ApartmentManagement.Application.Features.Auth.Commands.UpdateProfile;

public record UpdateProfileCommand(string FullName, string Email, string Phone) : IRequest<Result<UserDto>>;
