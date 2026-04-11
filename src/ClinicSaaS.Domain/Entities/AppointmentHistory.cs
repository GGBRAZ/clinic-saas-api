using ClinicSaaS.Domain.Enums;

namespace ClinicSaaS.Domain.Entities;

public class AppointmentHistory
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid AppointmentId { get; private set; }

    public AppointmentHistoryAction Action { get; private set; }

    public AppointmentStatus? OldStatus { get; private set; }
    public AppointmentStatus? NewStatus { get; private set; }

    public DateTime? OldDate { get; private set; }
    public DateTime? NewDate { get; private set; }

    public TimeSpan? OldStartTime { get; private set; }
    public TimeSpan? NewStartTime { get; private set; }

    public TimeSpan? OldEndTime { get; private set; }
    public TimeSpan? NewEndTime { get; private set; }

    public string? Notes { get; private set; }

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public Appointment Appointment { get; private set; } = null!;

    private AppointmentHistory() { }

    public AppointmentHistory(
        Guid appointmentId,
        AppointmentHistoryAction action,
        AppointmentStatus? oldStatus,
        AppointmentStatus? newStatus,
        DateTime? oldDate,
        DateTime? newDate,
        TimeSpan? oldStartTime,
        TimeSpan? newStartTime,
        TimeSpan? oldEndTime,
        TimeSpan? newEndTime,
        string? notes)
    {
        AppointmentId = appointmentId;
        Action = action;
        OldStatus = oldStatus;
        NewStatus = newStatus;
        OldDate = oldDate?.Date;
        NewDate = newDate?.Date;
        OldStartTime = oldStartTime;
        NewStartTime = newStartTime;
        OldEndTime = oldEndTime;
        NewEndTime = newEndTime;
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        CreatedAt = DateTime.UtcNow;
    }
}