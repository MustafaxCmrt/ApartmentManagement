using ApartmentManagement.Domain.Common;
using ApartmentManagement.Domain.Enums;

namespace ApartmentManagement.Domain.Entities;

public class MeetingParticipant : BaseEntity, ITenantEntity
{
    public Guid TenantId { get; set; }
    public Guid MeetingId { get; set; }
    public Guid ApartmentId { get; set; }
    public AttendanceStatus AttendanceStatus { get; set; }
    public Guid? ProxyApartmentId { get; set; }

    // Navigation
    public Meeting? Meeting { get; set; }
    public Apartment? Apartment { get; set; }
    public Apartment? ProxyApartment { get; set; }
}
