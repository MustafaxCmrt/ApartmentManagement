using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Residents.Commands.DeleteResident;

public class DeleteResidentHandler : IRequestHandler<DeleteResidentCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public DeleteResidentHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteResidentCommand request, CancellationToken ct)
    {
        var resident = await _db.Residents.FirstOrDefaultAsync(s => s.Id == request.Id, ct);
        if (resident is null)
            return Result.Failure(Error.NotFound("Resident"));

        _db.Residents.Remove(resident);
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
