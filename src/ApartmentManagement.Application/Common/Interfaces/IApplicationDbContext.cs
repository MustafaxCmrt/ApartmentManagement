using ApartmentManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Tenant> Tenants { get; }
    DbSet<User> Users { get; }
    DbSet<UserInvite> UserInvites { get; }
    DbSet<Building> Buildings { get; }
    DbSet<Apartment> Apartments { get; }
    DbSet<Resident> Residents { get; }
    DbSet<Due> Dues { get; }
    DbSet<DuePayment> DuePayments { get; }
    DbSet<Announcement> Announcements { get; }
    DbSet<AnnouncementRead> AnnouncementReads { get; }
    DbSet<MaintenanceTicket> MaintenanceTickets { get; }
    DbSet<MaintenanceComment> MaintenanceComments { get; }
    DbSet<Meeting> Meetings { get; }
    DbSet<MeetingParticipant> MeetingParticipants { get; }
    DbSet<Decision> Decisions { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<PasswordResetToken> PasswordResetTokens { get; }
    DbSet<Notification> Notifications { get; }

    Task<int> SaveChangesAsync(CancellationToken ct);
}
