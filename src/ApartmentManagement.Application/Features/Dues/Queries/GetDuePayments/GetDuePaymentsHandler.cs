using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Dues.Queries.GetDuePayments;

public class GetDuePaymentsHandler : IRequestHandler<GetDuePaymentsQuery, Result<List<DuePaymentDto>>>
{
    private readonly IApplicationDbContext _db;

    public GetDuePaymentsHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<List<DuePaymentDto>>> Handle(GetDuePaymentsQuery request, CancellationToken ct)
    {
        var dueExists = await _db.Dues.AnyAsync(d => d.Id == request.DueId, ct);
        if (!dueExists)
            return Result<List<DuePaymentDto>>.Failure(Error.NotFound("Due"));

        var payments = await _db.DuePayments
            .AsNoTracking()
            .Where(p => p.DueId == request.DueId)
            .OrderByDescending(p => p.PaymentDate)
            .Select(p => new DuePaymentDto
            {
                Id = p.Id,
                DueId = p.DueId,
                PaidAmount = p.PaidAmount,
                PaymentDate = p.PaymentDate,
                PaymentMethod = p.PaymentMethod.ToString(),
                Description = p.Description,
                ReceiptNumber = p.ReceiptNumber
            })
            .ToListAsync(ct);

        return Result<List<DuePaymentDto>>.Success(payments);
    }
}
