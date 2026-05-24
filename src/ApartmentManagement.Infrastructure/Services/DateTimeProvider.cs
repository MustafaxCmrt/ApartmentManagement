using ApartmentManagement.Application.Common.Interfaces;

namespace ApartmentManagement.Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
