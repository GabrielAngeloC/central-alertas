using CentralAlertas.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CentralAlertas.Infrastructure.Persistence;

public class CentralAlertasDbContext : DbContext
{
    public CentralAlertasDbContext(DbContextOptions<CentralAlertasDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Alert> Alerts => Set<Alert>();
    public DbSet<AlertOccurrence> AlertOccurrences => Set<AlertOccurrence>();
    public DbSet<Source> Sources => Set<Source>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Alert>(entity =>
        {
            entity.ToTable("alerts");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Source)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.Category)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.Type)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.Severity)
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(x => x.Title)
                .HasMaxLength(250)
                .IsRequired();

            entity.Property(x => x.Message)
                .HasMaxLength(1000);

            entity.Property(x => x.DedupKey)
                .HasMaxLength(250)
                .IsRequired();

            entity.Property(x => x.MetricValue)
                .HasPrecision(18, 4);

            entity.Property(x => x.MetricUnit)
                .HasMaxLength(50);

            entity.Property(x => x.MetricThreshold)
                .HasPrecision(18, 4);

            entity.Property(x => x.ItemsJson)
                .HasColumnType("jsonb");

            entity.Property(x => x.PayloadJson)
                .HasColumnType("jsonb");

            entity.HasIndex(x => new { x.Source, x.DedupKey })
                .IsUnique();

            entity.HasIndex(x => x.Category);
            entity.HasIndex(x => x.Type);
            entity.HasIndex(x => x.Severity);
            entity.HasIndex(x => x.LastSeenAt);
        });
        modelBuilder.Entity<Source>(entity =>
{
    entity.ToTable("sources");

    entity.HasKey(x => x.Id);

    entity.Property(x => x.Name)
        .HasMaxLength(150)
        .IsRequired();

    entity.Property(x => x.ExpectedIntervalMinutes)
        .IsRequired();

    entity.Property(x => x.IsActive)
        .IsRequired();

    entity.Property(x => x.CreatedAt)
        .IsRequired();

    entity.HasIndex(x => x.Name)
        .IsUnique();

    entity.HasIndex(x => x.IsActive);
    entity.HasIndex(x => x.LastReceivedAt);
});

        modelBuilder.Entity<AlertOccurrence>(entity =>
{
    entity.ToTable("alert_occurrences");

    entity.HasKey(x => x.Id);

    entity.Property(x => x.MetricValue)
        .HasPrecision(18, 4);

    entity.Property(x => x.MetricUnit)
        .HasMaxLength(50);

    entity.Property(x => x.MetricThreshold)
        .HasPrecision(18, 4);

    entity.Property(x => x.ItemsJson)
        .HasColumnType("jsonb");

    entity.Property(x => x.PayloadJson)
        .HasColumnType("jsonb");

    entity.Property(x => x.ReceivedAt)
        .IsRequired();

    entity.HasOne(x => x.Alert)
        .WithMany(x => x.Occurrences)
        .HasForeignKey(x => x.AlertId)
        .OnDelete(DeleteBehavior.Cascade);

    entity.HasIndex(x => x.AlertId);
    entity.HasIndex(x => x.ReceivedAt);
});
    }

}