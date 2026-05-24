using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Dues.Commands.UpdateDue;

public class UpdateDueHandler : IRequestHandler<UpdateDueCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public UpdateDueHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateDueCommand request, CancellationToken ct)
    {
        var due = await _db.Dues
            .Include(a => a.Payments)
            .FirstOrDefaultAsync(a => a.Id == request.Id, ct);

        if (due is null)
            return Result.Failure(Error.NotFound("Due"));

        due.Amount = request.Amount;
        due.DueDate = request.DueDate;
        due.Description = request.Description;

        // Update status (compare payment total against amount)
        var totalPaid = due.Payments.Sum(o => o.PaidAmount);
        if (totalPaid >= due.Amount) due.Status = DueStatus.Paid;
        else if (totalPaid > 0) due.Status = DueStatus.PartiallyPaid;
        else due.Status = DueStatus.Pending;

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
