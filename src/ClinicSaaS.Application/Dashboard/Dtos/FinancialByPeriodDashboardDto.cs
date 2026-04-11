namespace ClinicSaaS.Application.Dashboard.Dtos;

public class FinancialByPeriodDashboardDto
{
    public Guid ClinicId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public int TotalAppointments { get; set; }
    public int CompletedCount { get; set; }
    public int NoShowCount { get; set; }
    public int CanceledCount { get; set; }
    public int ScheduledCount { get; set; }

    public decimal TotalRevenue { get; set; }
    public decimal LostRevenue { get; set; }
    public decimal NoShowRate { get; set; }
}