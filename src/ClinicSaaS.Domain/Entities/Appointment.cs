using ClinicSaaS.Domain.Enums;

namespace ClinicSaaS.Domain.Entities;

public class Appointment
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid ClinicId { get; private set; }
    public Guid PatientId { get; private set; }

    public DateTime Date { get; private set; }
    public TimeSpan StartTime { get; private set; }
    public TimeSpan EndTime { get; private set; }

    public decimal ExpectedAmount { get; private set; }

    public AppointmentStatus Status { get; private set; } = AppointmentStatus.Scheduled;

    public string? Notes { get; private set; }

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public Clinic Clinic { get; private set; } = null!;
    public Patient Patient { get; private set; } = null!;

    private Appointment() { }

    public Appointment(
        Guid clinicId,
        Guid patientId,
        DateTime date,
        TimeSpan startTime,
        TimeSpan endTime,
        decimal expectedAmount,
        string? notes)
    {
        SetClinic(clinicId);
        SetPatient(patientId);
        SetSchedule(date, startTime, endTime);
        SetExpectedAmount(expectedAmount);
        SetNotes(notes);

        Status = AppointmentStatus.Scheduled;
        CreatedAt = DateTime.UtcNow;
    }

    public void SetClinic(Guid clinicId)
    {
        if (clinicId == Guid.Empty)
            throw new ArgumentException("ClinicId is required.");

        ClinicId = clinicId;
    }

    public void SetPatient(Guid patientId)
    {
        if (patientId == Guid.Empty)
            throw new ArgumentException("PatientId is required.");

        PatientId = patientId;
    }

    public void SetSchedule(DateTime date, TimeSpan start, TimeSpan end)
    {
        if (end <= start)
            throw new ArgumentException("End time must be greater than start time.");

        Date = date.Date;
        StartTime = start;
        EndTime = end;
    }

    public void SetExpectedAmount(decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative.");

        ExpectedAmount = amount;
    }

    public void SetNotes(string? notes)
    {
        Notes = string.IsNullOrWhiteSpace(notes)
            ? null
            : notes.Trim();
    }

    public void MarkAsCompleted()
    {
        Status = AppointmentStatus.Completed;
    }

    public void MarkAsCanceled()
    {
        Status = AppointmentStatus.Canceled;
    }

    public void MarkAsNoShow()
    {
        Status = AppointmentStatus.NoShow;
    }
}