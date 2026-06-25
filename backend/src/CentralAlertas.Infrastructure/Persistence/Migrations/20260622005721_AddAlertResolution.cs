using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CentralAlertas.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAlertResolution : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResolutionReason",
                table: "alerts",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResolvedAt",
                table: "alerts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_alerts_ResolvedAt",
                table: "alerts",
                column: "ResolvedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_alerts_ResolvedAt",
                table: "alerts");

            migrationBuilder.DropColumn(
                name: "ResolutionReason",
                table: "alerts");

            migrationBuilder.DropColumn(
                name: "ResolvedAt",
                table: "alerts");
        }
    }
}
