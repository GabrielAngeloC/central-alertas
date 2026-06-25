using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CentralAlertas.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAlertDeliveries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "alert_deliveries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AlertId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoutingRuleId = table.Column<Guid>(type: "uuid", nullable: true),
                    NotificationDestinationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Channel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ErrorMessage = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    AttemptedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alert_deliveries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_alert_deliveries_alerts_AlertId",
                        column: x => x.AlertId,
                        principalTable: "alerts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_alert_deliveries_notification_destinations_NotificationDest~",
                        column: x => x.NotificationDestinationId,
                        principalTable: "notification_destinations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_alert_deliveries_routing_rules_RoutingRuleId",
                        column: x => x.RoutingRuleId,
                        principalTable: "routing_rules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_alert_deliveries_AlertId",
                table: "alert_deliveries",
                column: "AlertId");

            migrationBuilder.CreateIndex(
                name: "IX_alert_deliveries_AttemptedAt",
                table: "alert_deliveries",
                column: "AttemptedAt");

            migrationBuilder.CreateIndex(
                name: "IX_alert_deliveries_Channel",
                table: "alert_deliveries",
                column: "Channel");

            migrationBuilder.CreateIndex(
                name: "IX_alert_deliveries_NotificationDestinationId",
                table: "alert_deliveries",
                column: "NotificationDestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_alert_deliveries_RoutingRuleId",
                table: "alert_deliveries",
                column: "RoutingRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_alert_deliveries_Status",
                table: "alert_deliveries",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "alert_deliveries");
        }
    }
}
