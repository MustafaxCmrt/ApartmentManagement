using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using ApartmentManagement.Domain.Entities;
using MediatR;

namespace ApartmentManagement.Application.Features.Buildings.Commands.CreateBuilding;

public class CreateBuildingHandler : IRequestHandler<CreateBuildingCommand, Result<BuildingDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentTenantService _currentTenant;

    public CreateBuildingHandler(IApplicationDbContext db, ICurrentTenantService currentTenant)
    {
        _db = db;
        _currentTenant = currentTenant;
    }

    public async Task<Result<BuildingDto>> Handle(CreateBuildingCommand request, CancellationToken ct)
    {
        if (_currentTenant.TenantId is not { } tenantId)
            return Result<BuildingDto>.Failure(Error.Unauthorized("Tenant is required."));

        var building = new Building
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = request.Name.Trim(),
            Address = request.Address.Trim(),
            FloorCount = request.FloorCount,
            ApartmentCount = request.ApartmentCount,
            ConstructionYear = request.ConstructionYear
        };

        _db.Buildings.Add(building);
        await _db.SaveChangesAsync(ct);

        return Result<BuildingDto>.Success(new BuildingDto
        {
            Id = building.Id,
            TenantId = building.TenantId,
            Name = building.Name,
            Address = building.Address,
            FloorCount = building.FloorCount,
            ApartmentCount = building.ApartmentCount,
            ConstructionYear = building.ConstructionYear,
            CreatedAt = building.CreatedAt
        });
    }
}
