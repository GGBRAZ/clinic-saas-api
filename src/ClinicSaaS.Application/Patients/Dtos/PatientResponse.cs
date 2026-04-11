namespace ClinicSaaS.Application.Patients.Dtos;

public class PatientResponse
{
    public Guid Id { get; set; }
    public Guid ClinicId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}