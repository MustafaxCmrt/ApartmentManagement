namespace ApartmentManagement.Domain.Exceptions;

public class TenantMismatchException : DomainException
{
    public TenantMismatchException() : base("Tenant ID değiştirilemez veya yetkisiz erişim tespit edildi.") { }
}
