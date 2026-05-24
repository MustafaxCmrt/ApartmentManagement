using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Dues.Commands.ReversePayment;

public class ReversePaymentHandler : IRequestHandler<ReversePaymentCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public ReversePaymentHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(ReversePaymentCommand request, CancellationToken ct)
    {
        var payment = await _db.DuePayments.FirstOrDefaultAsync(o => o.Id == request.PaymentId, ct);
        if (payment is null)
            return Result.Failure(Error.NotFound("Payment"));

        var due = await _db.Dues
            .Include(a => a.Payments)
            .FirstOrDefaultAsync(a => a.Id == payment.DueId, ct);

        if (due is null)
            return Result.Failure(Error.NotFound("Due"));

        _db.DuePayments.Remove(payment);

        var remainingPaid = due.Payments.Where(o => o.Id != payment.Id).Sum(o => o.PaidAmount);
        if (remainingPaid >= due.Amount) due.Status = DueStatus.Paid;
        else if (remainingPaid > 0) due.Status = DueStatus.PartiallyPaid;
        else due.Status = DueStatus.Pending;

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
