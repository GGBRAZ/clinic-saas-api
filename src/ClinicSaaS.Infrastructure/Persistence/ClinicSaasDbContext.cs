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
    }
}