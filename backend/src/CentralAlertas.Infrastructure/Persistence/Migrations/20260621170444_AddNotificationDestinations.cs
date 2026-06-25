using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CentralAlertas.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationDestinations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "notification_destinations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ConfigurationJson = table.Column<string>(type: "jsonb", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification_destinations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_notification_destinations_IsActive",
                table: "notification_destinations",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_notification_destinations_Name",
                table: "notification_destinations",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_notification_destinations_Type",
                table: "notification_destinations",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "notification_destinations");
        }
    }
}
