namespace ApartmentManagement.Application.Common.DTOs;

public class GlobalSearchResultDto
{
    public List<ResidentSearchItem> Residents { get; set; } = new();
    public List<ApartmentSearchItem> Apartments { get; set; } = new();
    public List<DueSearchItem> Dues { get; set; } = new();
}

public class ResidentSearchItem
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string ApartmentNumber { get; set; } = string.Empty;
    public string BuildingName { get; set; } = string.Empty;
}

public class ApartmentSearchItem
{
    public Guid Id { get; set; }
    public string ApartmentNumber { get; set; } = string.Empty;
    public string BuildingName { get; set; } = string.Empty;
    public int Floor { get; set; }
    public string OccupancyStatus { get; set; } = string.Empty;
}

public class DueSearchItem
{
    public Guid Id { get; set; }
    public string ApartmentNumber { get; set; } = string.Empty;
    public DateTime Period { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
}
