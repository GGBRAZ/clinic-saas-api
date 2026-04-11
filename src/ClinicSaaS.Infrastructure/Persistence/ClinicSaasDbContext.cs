using ClinicSaaS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicSaaS.Infrastructure.Persistence;

public class ClinicSaaSDbContext : DbContext
{
    public ClinicSaaSDbContext(DbContextOptions<ClinicSaaSDbContext> options)
        : base(options)
    {
    }

    public DbSet<Clinic> Clinics => Set<Clinic>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Appointment> Appointments => Set<Appointment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Clinic>(entity =>
        {
            entity.ToTable("Clinics");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(x => x.Slug)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(x => x.Phone)
                .IsRequired()
                .HasMaxLength(30);

            entity.Property(x => x.CreatedAt)
                .IsRequired();

            entity.HasIndex(x => x.Slug)
                .IsUnique();
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.ToTable("Patients");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.FullName)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(x => x.Phone)
                .IsRequired()
                .HasMaxLength(30);

            entity.Property(x => x.Email)
                .HasMaxLength(150);

            entity.Property(x => x.BirthDate)
                .HasColumnType("date");

            entity.Property(x => x.Notes)
                .HasMaxLength(1000);

            entity.Property(x => x.CreatedAt)
                .IsRequired();

            entity.HasOne(x => x.Clinic)
                .WithMany(x => x.Patients)
                .HasForeignKey(x => x.ClinicId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.ClinicId);
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.ToTable("Appointments");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Date)
                .HasColumnType("date")
                .IsRequired();

            entity.Property(x => x.StartTime)
                .IsRequired();

            entity.Property(x => x.EndTime)
                .IsRequired();

            entity.Property(x => x.ExpectedAmount)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            entity.Property(x => x.Status)
                .IsRequired();

            entity.Property(x => x.Notes)
                .HasMaxLength(1000);

            entity.Property(x => x.CreatedAt)
                .IsRequired();

            entity.HasOne(x => x.Clinic)
                .WithMany()
                .HasForeignKey(x => x.ClinicId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Patient)
                .WithMany()
                .HasForeignKey(x => x.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => x.ClinicId);
            entity.HasIndex(x => x.PatientId);
            entity.HasIndex(x => new { x.ClinicId, x.Date });
        });
    }
}