using System.Reflection;
using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Domain.Common;
using ApartmentManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Persistence.Contexts;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    protected readonly ICurrentTenantService _currentTenant;
    protected readonly ICurrentUserService _currentUser;
    protected readonly IDateTimeProvider _dateTime;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentTenantService currentTenant,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTime)
        : base(options)
    {
        _currentTenant = currentTenant;
        _currentUser = currentUser;
        _dateTime = dateTime;
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserInvite> UserInvites => Set<UserInvite>();
    public DbSet<Building> Buildings => Set<Building>();
    public DbSet<Apartment> Apartments => Set<Apartment>();
    public DbSet<Resident> Residents => Set<Resident>();
    public DbSet<Due> Dues => Set<Due>();
    public DbSet<DuePayment> DuePayments => Set<DuePayment>();
    public DbSet<Announcement> Announcements => Set<Announcement>();
    public DbSet<AnnouncementRead> AnnouncementReads => Set<AnnouncementRead>();
    public DbSet<MaintenanceTicket> MaintenanceTickets => Set<MaintenanceTicket>();
    public DbSet<MaintenanceComment> MaintenanceComments => Set<MaintenanceComment>();
    public DbSet<Meeting> Meetings => Set<Meeting>();
    public DbSet<MeetingParticipant> MeetingParticipants => Set<MeetingParticipant>();
    public DbSet<Decision> Decisions => Set<Decision>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    private static readonly MethodInfo TenantAndSoftDeleteMethod =
        typeof(ApplicationDbContext).GetMethod(
            nameof(SetTenantAndSoftDeleteFilter),
            BindingFlags.NonPublic | BindingFlags.Instance)!;

    private static readonly MethodInfo TenantOnlyMethod =
        typeof(ApplicationDbContext).GetMethod(
            nameof(SetTenantOnlyFilter),
            BindingFlags.NonPublic | BindingFlags.Instance)!;

    private static readonly MethodInfo SoftDeleteOnlyMethod =
        typeof(ApplicationDbContext).GetMethod(
            nameof(SetSoftDeleteOnlyFilter),
            BindingFlags.NonPublic | BindingFlags.Instance)!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        ApplyGlobalFilters(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    private void ApplyGlobalFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;
            var isTenant = typeof(ITenantEntity).IsAssignableFrom(clrType);
            var isSoftDelete = typeof(ISoftDeletable).IsAssignableFrom(clrType);

            if (isTenant && isSoftDelete)
            {
                TenantAndSoftDeleteMethod.MakeGenericMethod(clrType)
                    .Invoke(this, new object[] { modelBuilder });
            }
            else if (isTenant)
            {
                TenantOnlyMethod.MakeGenericMethod(clrType)
                    .Invoke(this, new object[] { modelBuilder });
            }
            else if (isSoftDelete)
            {
                SoftDeleteOnlyMethod.MakeGenericMethod(clrType)
                    .Invoke(this, new object[] { modelBuilder });
            }
        }
    }

    private void SetTenantAndSoftDeleteFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : class, ITenantEntity, ISoftDeletable
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e =>
            (_currentTenant.IsSuperAdmin || e.TenantId == _currentTenant.TenantId!.Value)
            && !e.IsDeleted);
    }

    private void SetTenantOnlyFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : class, ITenantEntity
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e =>
            _currentTenant.IsSuperAdmin || e.TenantId == _currentTenant.TenantId!.Value);
    }

    private void SetSoftDeleteOnlyFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : class, ISoftDeletable
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e => !e.IsDeleted);
    }
}
