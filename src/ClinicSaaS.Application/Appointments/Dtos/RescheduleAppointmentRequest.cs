namespace ClinicSaaS.Application.Appointments.Dtos;

public class RescheduleAppointmentRequest
{
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string? Notes { get; set; }
}