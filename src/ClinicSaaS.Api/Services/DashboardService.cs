using ClinicSaaS.Application.Dashboard.Dtos;
using ClinicSaaS.Domain.Enums;
using ClinicSaaS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClinicSaaS.Api.Services;

public class DashboardService
{
    private readonly ClinicSaaSDbContext _context;

    public DashboardService(ClinicSaaSDbContext context)
    {
        _context = context;
    }

    public async Task<FinancialDashboardDto> GetFinancial(Guid clinicId)
    {
        var appointments = await _context.Appointments
            .Where(x => x.ClinicId == clinicId)
            .ToListAsync();

        var completed = appointments
            .Where(x => x.Status == AppointmentStatus.Completed);

        var noShows = appointments
            .Where(x => x.Status == AppointmentStatus.NoShow);

        return new FinancialDashboardDto
        {
            TotalRevenue = completed.Sum(x => x.ExpectedAmount),
            LostRevenue = noShows.Sum(x => x.ExpectedAmount),
            CompletedCount = completed.Count(),
            NoShowCount = noShows.Count()
        };
    }
    public async Task<FinancialByPeriodDashboardDto> GetFinancialByPeriod(
       Guid clinicId,
       DateTime startDate,
       DateTime endDate)
    {
        var normalizedStart = startDate.Date;
        var normalizedEnd = endDate.Date;

        var appointments = await _context.Appointments
            .Where(x =>
                x.ClinicId == clinicId &&
                x.Date >= normalizedStart &&
                x.Date <= normalizedEnd)
            .ToListAsync();

        var totalAppointments = appointments.Count;
        var completedCount = appointments.Count(x => x.Status == AppointmentStatus.Completed);
        var noShowCount = appointments.Count(x => x.Status == AppointmentStatus.NoShow);
        var canceledCount = appointments.Count(x => x.Status == AppointmentStatus.Canceled);
        var scheduledCount = appointments.Count(x => x.Status == AppointmentStatus.Scheduled);

        var totalRevenue = appointments
            .Where(x => x.Status == AppointmentStatus.Completed)
            .Sum(x => x.ExpectedAmount);

        var lostRevenue = appointments
            .Where(x => x.Status == AppointmentStatus.NoShow)
            .Sum(x => x.ExpectedAmount);

        var noShowRate = totalAppointments == 0
            ? 0
            : Math.Round((decimal)noShowCount / totalAppointments * 100, 2);

        return new FinancialByPeriodDashboardDto
        {
            ClinicId = clinicId,
            StartDate = normalizedStart,
            EndDate = normalizedEnd,
            TotalAppointments = totalAppointments,
            CompletedCount = completedCount,
            NoShowCount = noShowCount,
            CanceledCount = canceledCount,
            ScheduledCount = scheduledCount,
            TotalRevenue = totalRevenue,
            LostRevenue = lostRevenue,
            NoShowRate = noShowRate
        };
    }
}