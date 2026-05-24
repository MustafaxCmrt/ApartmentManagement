using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Dues.Queries.GetDueById;

public class GetDueByIdHandler : IRequestHandler<GetDueByIdQuery, Result<DueDetailDto>>
{
    private readonly IApplicationDbContext _db;

    public GetDueByIdHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<DueDetailDto>> Handle(GetDueByIdQuery request, CancellationToken ct)
    {
        var dto = await _db.Dues.AsNoTracking()
            .Where(a => a.Id == request.Id)
            .Select(a => new DueDetailDto
            {
                Id = a.Id,
                TenantId = a.TenantId,
                ApartmentId = a.ApartmentId,
                ApartmentNumber = a.Apartment != null ? a.Apartment.ApartmentNumber : null,
                BuildingName = a.Apartment != null && a.Apartment.Building != null ? a.Apartment.Building.Name : null,
                Period = a.Period,
                Amount = a.Amount,
                DueDate = a.DueDate,
                Description = a.Description,
                Status = a.Status.ToString(),
                CreationType = a.CreationType.ToString(),
                TotalPaid = a.Payments.Sum(o => (decimal?)o.PaidAmount) ?? 0m,
                RemainingAmount = a.Amount - (a.Payments.Sum(o => (decimal?)o.PaidAmount) ?? 0m),
                Payments = a.Payments
                    .OrderByDescending(o => o.PaymentDate)
                    .Select(o => new DuePaymentDto
                    {
                        Id = o.Id,
                        DueId = o.DueId,
                        PaidAmount = o.PaidAmount,
                        PaymentDate = o.PaymentDate,
                        PaymentMethod = o.PaymentMethod.ToString(),
                        Description = o.Description,
                        ReceiptNumber = o.ReceiptNumber
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(ct);

        if (dto is null)
            return Result<DueDetailDto>.Failure(Error.NotFound("Due"));

        return Result<DueDetailDto>.Success(dto);
    }
}
