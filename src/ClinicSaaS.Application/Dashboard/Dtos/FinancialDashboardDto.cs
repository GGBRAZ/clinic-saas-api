namespace ClinicSaaS.Application.Dashboard.Dtos;

public class FinancialDashboardDto
{
    public decimal TotalRevenue { get; set; }
    public decimal LostRevenue { get; set; }
    public int NoShowCount { get; set; }
    public int CompletedCount { get; set; }
}