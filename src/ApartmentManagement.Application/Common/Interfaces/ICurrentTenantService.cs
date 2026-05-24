namespace ApartmentManagement.Application.Common.Interfaces;

public interface ICurrentTenantService
{
    Guid? TenantId { get; }
    bool IsSuperAdmin { get; }
}
