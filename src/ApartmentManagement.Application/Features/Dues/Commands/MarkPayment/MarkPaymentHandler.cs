using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using ApartmentManagement.Domain.Entities;
using ApartmentManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Dues.Commands.MarkPayment;

public class MarkPaymentHandler : IRequestHandler<MarkPaymentCommand, Result<DuePaymentDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentTenantService _currentTenant;

    public MarkPaymentHandler(IApplicationDbContext db, ICurrentTenantService currentTenant)
    {
        _db = db;
        _currentTenant = currentTenant;
    }

    public async Task<Result<DuePaymentDto>> Handle(MarkPaymentCommand request, CancellationToken ct)
    {
        if (_currentTenant.TenantId is not { } tenantId)
            return Result<DuePaymentDto>.Failure(Error.Unauthorized("Tenant is required."));

        if (!Enum.TryParse<PaymentMethod>(request.PaymentMethod, true, out var method))
            return Result<DuePaymentDto>.Failure(Error.Validation("Invalid payment method."));

        var due = await _db.Dues
            .Include(a => a.Payments)
            .FirstOrDefaultAsync(a => a.Id == request.DueId, ct);

        if (due is null)
            return Result<DuePaymentDto>.Failure(Error.NotFound("Due"));

        var currentPaid = due.Payments.Sum(o => o.PaidAmount);
        var newTotal = currentPaid + request.PaidAmount;

        if (newTotal > due.Amount + 0.01m)
            return Result<DuePaymentDto>.Failure(Error.Validation("Payment amount exceeds the outstanding balance."));

        var payment = new DuePayment
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            DueId = due.Id,
            PaidAmount = request.PaidAmount,
            PaymentDate = request.PaymentDate,
            PaymentMethod = method,
            Description = request.Description,
            ReceiptNumber = request.ReceiptNumber
        };

        _db.DuePayments.Add(payment);

        if (newTotal >= due.Amount) due.Status = DueStatus.Paid;
        else if (newTotal > 0) due.Status = DueStatus.PartiallyPaid;

        await _db.SaveChangesAsync(ct);

        return Result<DuePaymentDto>.Success(new DuePaymentDto
        {
            Id = payment.Id,
            DueId = payment.DueId,
            PaidAmount = payment.PaidAmount,
            PaymentDate = payment.PaymentDate,
            PaymentMethod = payment.PaymentMethod.ToString(),
            Description = payment.Description,
            ReceiptNumber = payment.ReceiptNumber
        });
    }
}
