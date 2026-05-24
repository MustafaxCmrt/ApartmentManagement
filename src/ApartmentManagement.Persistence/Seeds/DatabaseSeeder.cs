using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Utilities;
using ApartmentManagement.Domain.Entities;
using ApartmentManagement.Domain.Enums;
using ApartmentManagement.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ApartmentManagement.Persistence.Seeds;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider sp)
    {
        using var scope = sp.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        var now = DateTime.UtcNow;

        // ---------- SuperAdmin ----------
        var superAdminEmail = "admin@admin.com";
        var superAdminPhone = PhoneNormalizer.Normalize("+90 555 000 0001");
        var superAdmin = await context.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email == superAdminEmail && u.TenantId == null);

        if (superAdmin is null)
        {
            superAdmin = new User
            {
                Id = Guid.NewGuid(),
                TenantId = null,
                Email = superAdminEmail,
                PasswordHash = passwordHasher.Hash("Admin44."),
                FullName = "System Administrator",
                Phone = superAdminPhone,
                Role = UserRole.SuperAdmin,
                IsActive = true,
                IsEmailVerified = true,
                CreatedAt = now
            };
            context.Users.Add(superAdmin);
            await context.SaveChangesAsync(CancellationToken.None);
        }
        else if (string.IsNullOrWhiteSpace(superAdmin.Phone))
        {
            superAdmin.Phone = superAdminPhone;
            superAdmin.UpdatedAt = now;
            await context.SaveChangesAsync(CancellationToken.None);
        }

        // ---------- Demo Tenant ----------
        var demoTenant = await context.Tenants
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.ShortName == "demo");

        if (demoTenant is null)
        {
            demoTenant = new Tenant
            {
                Id = Guid.NewGuid(),
                Name = "Demo Building",
                ShortName = "demo",
                IsActive = true,
                SubscriptionStart = now,
                SubscriptionEnd = null,
                MaxApartmentCount = 50,
                ContactEmail = "contact@demo.com",
                ContactPhone = PhoneNormalizer.Normalize("+90 555 000 0000"),
                Address = "Demo District, Demo Street No:1, Istanbul",
                CreatedAt = now,
                UpdatedAt = now
            };
            context.Tenants.Add(demoTenant);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        // ---------- Building (A Block) ----------
        var aBlock = await context.Buildings
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.TenantId == demoTenant.Id && x.Name == "A Block");

        if (aBlock is null)
        {
            aBlock = new Building
            {
                Id = Guid.NewGuid(),
                TenantId = demoTenant.Id,
                Name = "A Block",
                Address = "Demo District, Demo Street No:1, Istanbul",
                FloorCount = 5,
                ApartmentCount = 10,
                ConstructionYear = 2015,
                CreatedAt = now
            };
            context.Buildings.Add(aBlock);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        // ---------- Apartments (10 units) ----------
        var existingApartments = await context.Apartments
            .IgnoreQueryFilters()
            .Where(d => d.TenantId == demoTenant.Id && d.BuildingId == aBlock.Id)
            .ToListAsync();

        var apartments = new List<Apartment>(existingApartments);

        if (existingApartments.Count < 10)
        {
            for (int floor = 1; floor <= 5; floor++)
            {
                for (int no = 1; no <= 2; no++)
                {
                    var apartmentNumber = $"{floor}{no}";
                    if (existingApartments.Any(d => d.ApartmentNumber == apartmentNumber))
                        continue;

                    var d = new Apartment
                    {
                        Id = Guid.NewGuid(),
                        TenantId = demoTenant.Id,
                        BuildingId = aBlock.Id,
                        ApartmentNumber = apartmentNumber,
                        Floor = floor,
                        GrossSquareMeters = 110m,
                        NetSquareMeters = 95m,
                        OccupancyStatus = OccupancyStatus.Occupied,
                        DueMultiplier = 1.0m,
                        CreatedAt = now
                    };
                    context.Apartments.Add(d);
                    apartments.Add(d);
                }
            }
            await context.SaveChangesAsync(CancellationToken.None);
        }

        // ---------- TenantAdmin ----------
        var tenantAdminEmail = "yonetici@demo.com";
        var tenantAdmin = await context.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email == tenantAdminEmail && u.TenantId == demoTenant.Id);

        if (tenantAdmin is null)
        {
            tenantAdmin = new User
            {
                Id = Guid.NewGuid(),
                TenantId = demoTenant.Id,
                Email = tenantAdminEmail,
                PasswordHash = passwordHasher.Hash("Demo44."),
                FullName = "Demo Admin",
                Phone = PhoneNormalizer.Normalize("+90 555 111 2233"),
                Role = UserRole.TenantAdmin,
                IsActive = true,
                IsEmailVerified = true,
                CreatedAt = now
            };
            context.Users.Add(tenantAdmin);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        // ---------- Resident user + Resident ----------
        var residentEmail = "sakin@demo.com";
        var resident = await context.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email == residentEmail && u.TenantId == demoTenant.Id);

        if (resident is null)
        {
            resident = new User
            {
                Id = Guid.NewGuid(),
                TenantId = demoTenant.Id,
                Email = residentEmail,
                PasswordHash = passwordHasher.Hash("Demo44."),
                FullName = "Demo Resident",
                Phone = PhoneNormalizer.Normalize("+90 555 444 5566"),
                Role = UserRole.Resident,
                IsActive = true,
                IsEmailVerified = true,
                CreatedAt = now
            };
            context.Users.Add(resident);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        var firstApartment = apartments.OrderBy(d => d.Floor).ThenBy(d => d.ApartmentNumber).FirstOrDefault();
        if (firstApartment is not null)
        {
            var existingResident = await context.Residents
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(s =>
                    s.TenantId == demoTenant.Id &&
                    s.ApartmentId == firstApartment.Id &&
                    s.UserId == resident.Id);

            if (existingResident is null)
            {
                var residentEntity = new Resident
                {
                    Id = Guid.NewGuid(),
                    TenantId = demoTenant.Id,
                    ApartmentId = firstApartment.Id,
                    UserId = resident.Id,
                    FullName = resident.FullName,
                    Phone = resident.Phone,
                    Email = resident.Email,
                    ResidentType = ResidentType.Owner,
                    MoveInDate = now,
                    IsPrimaryContact = true,
                    CreatedAt = now
                };
                context.Residents.Add(residentEntity);
                await context.SaveChangesAsync(CancellationToken.None);
            }
        }
    }
}
