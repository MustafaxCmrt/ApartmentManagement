namespace ApartmentManagement.Application.Common.DTOs;

public class DueDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ApartmentId { get; set; }
    public string? ApartmentNumber { get; set; }
    public string? BuildingName { get; set; }
    public DateTime Period { get; set; }
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public string CreationType { get; set; } = string.Empty;
    public decimal TotalPaid { get; set; }
    public decimal RemainingAmount { get; set; }
}

public class DueDetailDto : DueDto
{
    public List<DuePaymentDto> Payments { get; set; } = new();
}

public class DuePaymentDto
{
    public Guid Id { get; set; }
    public Guid DueId { get; set; }
    public decimal PaidAmount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ReceiptNumber { get; set; }
}

public class OverdueDueDto
{
    public Guid Id { get; set; }
    public Guid ApartmentId { get; set; }
    public string ApartmentNumber { get; set; } = string.Empty;
    public string? BuildingName { get; set; }
    public DateTime Period { get; set; }
    public decimal Amount { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal RemainingAmount { get; set; }
    public DateTime DueDate { get; set; }
    public int OverdueDays { get; set; }
}

public class MonthlyReportDto
{
    public DateTime Period { get; set; }
    public int TotalDueCount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal CollectedAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public int PaidDueCount { get; set; }
    public int PendingDueCount { get; set; }
    public int OverdueDueCount { get; set; }
    public int PartiallyPaidCount { get; set; }
}

public class ApartmentSummaryDto
{
    public Guid ApartmentId { get; set; }
    public string ApartmentNumber { get; set; } = string.Empty;
    public string? BuildingName { get; set; }
    public int TotalDueCount { get; set; }
    public decimal TotalDebt { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal RemainingDebt { get; set; }
    public int OverdueDueCount { get; set; }
}
