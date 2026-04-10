namespace ClinicSaaS.Domain.Entities;

public class Clinic
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private Clinic() { }

    public Clinic(string name, string slug, string email, string phone)
    {
        Name = name;
        Slug = slug;
        Email = email;
        Phone = phone;
    }
}