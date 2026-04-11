namespace ClinicSaaS.Application.Patients.Dtos;

public class CreatePatientRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Notes { get; set; }
}