using ApartmentManagement.Application.Common.Models;
using MediatR;

namespace ApartmentManagement.Application.Features.Meetings.Commands.UpdateMinutes;

public record UpdateMinutesCommand(Guid Id, string MinutesSummary) : IRequest<Result>;
