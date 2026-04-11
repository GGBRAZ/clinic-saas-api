namespace ClinicSaaS.Domain.Entities;

public class Patient
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ClinicId { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public DateTime? BirthDate { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public Clinic Clinic { get; private set; } = null!;

    private Patient() { }

    public Patient(
        Guid clinicId,
        string fullName,
        string phone,
        string? email,
        DateTime? birthDate,
        string? notes)
    {
        SetClinicId(clinicId);
        SetFullName(fullName);
        SetPhone(phone);
        SetEmail(email);
        SetBirthDate(birthDate);
        SetNotes(notes);
        CreatedAt = DateTime.UtcNow;
    }

    public void SetClinicId(Guid clinicId)
    {
        if (clinicId == Guid.Empty)
            throw new ArgumentException("ClinicId is required.");

        ClinicId = clinicId;
    }

    public void SetFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Patient full name is required.");

        FullName = fullName.Trim();
    }

    public void SetPhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Patient phone is required.");

        Phone = phone.Trim();
    }

    public void SetEmail(string? email)
    {
        Email = string.IsNullOrWhiteSpace(email)
            ? null
            : email.Trim().ToLowerInvariant();
    }

    public void SetBirthDate(DateTime? birthDate)
    {
        BirthDate = birthDate;
    }

    public void SetNotes(string? notes)
    {
        Notes = string.IsNullOrWhiteSpace(notes)
            ? null
            : notes.Trim();
    }
}