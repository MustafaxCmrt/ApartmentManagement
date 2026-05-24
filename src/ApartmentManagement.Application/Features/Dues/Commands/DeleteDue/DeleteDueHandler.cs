using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Dues.Commands.DeleteDue;

public class DeleteDueHandler : IRequestHandler<DeleteDueCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public DeleteDueHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteDueCommand request, CancellationToken ct)
    {
        var due = await _db.Dues.FirstOrDefaultAsync(a => a.Id == request.Id, ct);
        if (due is null)
            return Result.Failure(Error.NotFound("Due"));

        var hasPayment = await _db.DuePayments.AnyAsync(o => o.DueId == request.Id, ct);
        if (hasPayment)
            return Result.Failure(Error.Conflict("This due has linked payments. Delete the payments first."));

        _db.Dues.Remove(due);
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
