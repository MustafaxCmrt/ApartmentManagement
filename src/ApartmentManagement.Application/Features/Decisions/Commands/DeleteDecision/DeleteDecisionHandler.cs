using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Decisions.Commands.DeleteDecision;

public class DeleteDecisionHandler : IRequestHandler<DeleteDecisionCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public DeleteDecisionHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteDecisionCommand request, CancellationToken ct)
    {
        var decision = await _db.Decisions.FirstOrDefaultAsync(k => k.Id == request.Id, ct);
        if (decision is null)
            return Result.Failure(Error.NotFound("Decision"));

        _db.Decisions.Remove(decision);
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
