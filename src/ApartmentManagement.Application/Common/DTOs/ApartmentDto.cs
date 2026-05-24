namespace ApartmentManagement.Application.Common.DTOs;

public class ApartmentDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid BuildingId { get; set; }
    public string? BuildingName { get; set; }
    public string ApartmentNumber { get; set; } = string.Empty;
    public int Floor { get; set; }
    public decimal? GrossSquareMeters { get; set; }
    public decimal? NetSquareMeters { get; set; }
    public string OccupancyStatus { get; set; } = string.Empty;
    public decimal DueMultiplier { get; set; }
}

public class ApartmentDetailDto : ApartmentDto
{
    public int ActiveResidentCount { get; set; }
    public int TotalDueCount { get; set; }
    public int PendingDueCount { get; set; }
}

public class ApartmentCreateItem
{
    public string ApartmentNumber { get; set; } = string.Empty;
    public int Floor { get; set; }
    public decimal? GrossSquareMeters { get; set; }
    public decimal? NetSquareMeters { get; set; }
    public decimal DueMultiplier { get; set; } = 1.0m;
}
