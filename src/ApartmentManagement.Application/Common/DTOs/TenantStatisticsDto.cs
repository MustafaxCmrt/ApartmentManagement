namespace ApartmentManagement.Application.Common.DTOs;

public class TenantStatisticsDto
{
    public Guid TenantId { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public int TotalBuildingCount { get; set; }
    public int TotalApartmentCount { get; set; }
    public int TotalResidentCount { get; set; }
    public int ActiveUserCount { get; set; }
    public int PendingDueCount { get; set; }
    public int OverdueDueCount { get; set; }
    public decimal TotalCollectedAmount { get; set; }
}
