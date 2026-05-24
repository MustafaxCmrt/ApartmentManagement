namespace ApartmentManagement.Application.Common.DTOs;

public class BuildingDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int FloorCount { get; set; }
    public int ApartmentCount { get; set; }
    public int? ConstructionYear { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class BuildingDetailDto : BuildingDto
{
    public int RegisteredApartmentCount { get; set; }
    public int ActiveResidentCount { get; set; }
}
