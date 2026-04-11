namespace ClinicSaaS.Application.Appointments.Dtos;

public class CreateAppointmentRequest
{
    public Guid ClinicId { get; set; }
    public Guid PatientId { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public decimal ExpectedAmount { get; set; }
    public string? Notes { get; set; }
}