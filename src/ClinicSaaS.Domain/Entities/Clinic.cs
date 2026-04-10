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
        SetName(name);
        SetSlug(slug);
        SetEmail(email);
        SetPhone(phone);
        CreatedAt = DateTime.UtcNow;
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Clinic name is required.");

        Name = name.Trim();
    }

    public void SetSlug(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            throw new ArgumentException("Clinic slug is required.");

        Slug = slug.Trim().ToLowerInvariant();
    }

    public void SetEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Clinic email is required.");

        Email = email.Trim().ToLowerInvariant();
    }

    public void SetPhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Clinic phone is required.");

        Phone = phone.Trim();
    }
}