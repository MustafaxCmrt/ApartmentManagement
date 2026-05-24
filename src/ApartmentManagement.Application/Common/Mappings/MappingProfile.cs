using ApartmentManagement.Application.Common.DTOs;
using ApartmentManagement.Domain.Entities;
using AutoMapper;

namespace ApartmentManagement.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Auth
        CreateMap<User, UserDto>();
        CreateMap<User, LoggedInUserDto>();

        // Tenant
        CreateMap<Tenant, TenantDto>();

        // Building
        CreateMap<Building, BuildingDto>();
        CreateMap<Building, BuildingDetailDto>();

        // Apartment
        CreateMap<Apartment, ApartmentDto>();
        CreateMap<Apartment, ApartmentDetailDto>();

        // Resident
        CreateMap<Resident, ResidentDto>();
        CreateMap<Resident, ResidentDetailDto>();

        // Due
        CreateMap<Due, DueDto>();
        CreateMap<Due, DueDetailDto>();
        CreateMap<DuePayment, DuePaymentDto>();

        // Announcement
        CreateMap<Announcement, AnnouncementDto>();
        CreateMap<Announcement, AnnouncementDetailDto>();

        // Maintenance
        CreateMap<MaintenanceTicket, MaintenanceTicketDto>();
        CreateMap<MaintenanceTicket, MaintenanceTicketDetailDto>();
        CreateMap<MaintenanceComment, MaintenanceCommentDto>();

        // Meeting
        CreateMap<Meeting, MeetingDto>();
        CreateMap<Meeting, MeetingDetailDto>();
        CreateMap<MeetingParticipant, ParticipantDto>();

        // Decision
        CreateMap<Decision, DecisionDto>();
    }
}
