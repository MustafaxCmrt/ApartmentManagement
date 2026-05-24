namespace ApartmentManagement.Application.Common.DTOs;

public class DashboardSummaryDto
{
    public int TotalApartments { get; set; }
    public int ActiveResidentCount { get; set; }
    public int OpenMaintenanceTicketCount { get; set; }
    public int OverdueDueCount { get; set; }
    public decimal MonthlyCollectedAmount { get; set; }
}

public class DuesTrendDto
{
    public List<DuesTrendItem> Items { get; set; } = new();
}

public class DuesTrendItem
{
    public DateTime Period { get; set; }
    public string PeriodLabel { get; set; } = string.Empty;
    public decimal Collected { get; set; }
    public decimal Expected { get; set; }
}

public class ActivityDto
{
    public string Type { get; set; } = string.Empty; // Announcement, DuePayment, MaintenanceTicket
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime Date { get; set; }
}
