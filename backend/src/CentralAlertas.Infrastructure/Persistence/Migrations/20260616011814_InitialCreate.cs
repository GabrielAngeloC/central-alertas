using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CentralAlertas.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "alerts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Source = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Severity = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Title = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Message = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    DedupKey = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    MetricValue = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    MetricUnit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    MetricThreshold = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    ItemsJson = table.Column<string>(type: "jsonb", nullable: true),
                    PayloadJson = table.Column<string>(type: "jsonb", nullable: true),
                    OccurrenceCount = table.Column<int>(type: "integer", nullable: false),
                    FirstSeenAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastSeenAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastNotifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsEscalating = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alerts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_alerts_Category",
                table: "alerts",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_alerts_LastSeenAt",
                table: "alerts",
                column: "LastSeenAt");

            migrationBuilder.CreateIndex(
                name: "IX_alerts_Severity",
                table: "alerts",
                column: "Severity");

            migrationBuilder.CreateIndex(
                name: "IX_alerts_Source_DedupKey",
                table: "alerts",
                columns: new[] { "Source", "DedupKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_alerts_Type",
                table: "alerts",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "alerts");
        }
    }
}
