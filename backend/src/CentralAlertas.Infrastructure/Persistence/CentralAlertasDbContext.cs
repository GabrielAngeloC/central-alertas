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
    public DbSet<NotificationDestination> NotificationDestinations => Set<NotificationDestination>();
    public DbSet<RoutingRule> RoutingRules => Set<RoutingRule>();
    public DbSet<RoutingRuleDestination> RoutingRuleDestinations => Set<RoutingRuleDestination>();
    public DbSet<AlertDelivery> AlertDeliveries => Set<AlertDelivery>();
    public DbSet<AppUser> AppUsers => Set<AppUser>();
    public DbSet<AllowedOrigin> AllowedOrigins => Set<AllowedOrigin>();
    public DbSet<DashboardViewConfig> DashboardViews => Set<DashboardViewConfig>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<AlertDelivery>(entity =>
        {
            entity.ToTable("alert_deliveries");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Channel)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.Status)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.ErrorMessage)
                .HasMaxLength(2000);

            entity.Property(x => x.AttemptedAt)
                .IsRequired();

            entity.HasOne(x => x.Alert)
                .WithMany()
                .HasForeignKey(x => x.AlertId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.RoutingRule)
                .WithMany()
                .HasForeignKey(x => x.RoutingRuleId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(x => x.NotificationDestination)
                .WithMany()
                .HasForeignKey(x => x.NotificationDestinationId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(x => x.AlertId);
            entity.HasIndex(x => x.RoutingRuleId);
            entity.HasIndex(x => x.NotificationDestinationId);
            entity.HasIndex(x => x.Channel);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.AttemptedAt);
        });
        modelBuilder.Entity<RoutingRule>(entity =>
        {
            entity.ToTable("routing_rules");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.Order)
                .HasColumnName("rule_order")
                .IsRequired();

            entity.Property(x => x.Severity)
                .HasMaxLength(30);

            entity.Property(x => x.Category)
                .HasMaxLength(100);

            entity.Property(x => x.Type)
                .HasMaxLength(150);

            entity.Property(x => x.Source)
                .HasMaxLength(150);

            entity.Property(x => x.DeliveryMode)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.IsActive)
                .IsRequired();

            entity.Property(x => x.CreatedAt)
                .IsRequired();

            entity.HasIndex(x => x.Order);
            entity.HasIndex(x => x.IsActive);
            entity.HasIndex(x => x.Severity);
            entity.HasIndex(x => x.Category);
            entity.HasIndex(x => x.Type);
            entity.HasIndex(x => x.Source);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<RoutingRuleDestination>(entity =>
        {
            entity.ToTable("routing_rule_destinations");

            entity.HasKey(x => x.Id);

            entity.HasOne(x => x.RoutingRule)
                .WithMany(x => x.Destinations)
                .HasForeignKey(x => x.RoutingRuleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.NotificationDestination)
                .WithMany()
                .HasForeignKey(x => x.NotificationDestinationId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => x.RoutingRuleId);
            entity.HasIndex(x => x.NotificationDestinationId);

            entity.HasIndex(x => new
            {
                x.RoutingRuleId,
                x.NotificationDestinationId
            }).IsUnique();
        });
        modelBuilder.Entity<NotificationDestination>(entity =>
        {
            entity.ToTable("notification_destinations");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.Type)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.ConfigurationJson)
                .HasColumnType("jsonb")
                .IsRequired();

            entity.Property(x => x.IsActive)
                .IsRequired();

            entity.Property(x => x.CreatedAt)
                .IsRequired();

            entity.HasIndex(x => x.Name)
                .IsUnique();

            entity.HasIndex(x => x.Type);
            entity.HasIndex(x => x.IsActive);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });
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

            entity.Property(x => x.ResolutionReason)
            .HasMaxLength(1000);

            entity.HasIndex(x => x.ResolvedAt);

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

            entity.HasQueryFilter(x => !x.IsDeleted);
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

    modelBuilder.Entity<AppUser>(entity =>
        {
            entity.ToTable("app_users");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.Email)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.PasswordHash)
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(x => x.IsActive)
                .IsRequired();

            entity.Property(x => x.CreatedAt)
                .IsRequired();

            entity.HasIndex(x => x.Email)
                .IsUnique();

            entity.HasIndex(x => x.IsActive);
        });

        modelBuilder.Entity<AllowedOrigin>(entity =>
        {
            entity.ToTable("allowed_origins");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Origin)
                .HasMaxLength(300)
                .IsRequired();

            entity.Property(x => x.Description)
                .HasMaxLength(300);

            entity.Property(x => x.IsActive)
                .IsRequired();

            entity.Property(x => x.CreatedAt)
                .IsRequired();

            entity.HasIndex(x => x.Origin)
                .IsUnique();

            entity.HasIndex(x => x.IsActive);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<DashboardViewConfig>(entity =>
        {
            entity.ToTable("dashboard_views");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Category)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.Title)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.Order)
                .HasColumnName("view_order")
                .IsRequired();

            entity.Property(x => x.IsActive)
                .IsRequired();

            entity.Property(x => x.CreatedAt)
                .IsRequired();

            entity.HasIndex(x => x.Category)
                .IsUnique();

            entity.HasIndex(x => x.Order);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });
    }

}