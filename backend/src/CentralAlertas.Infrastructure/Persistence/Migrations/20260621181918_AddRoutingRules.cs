using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CentralAlertas.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRoutingRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "routing_rules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    rule_order = table.Column<int>(type: "integer", nullable: false),
                    Severity = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Type = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Source = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    DeliveryMode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ThrottleMinutes = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_routing_rules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "routing_rule_destinations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoutingRuleId = table.Column<Guid>(type: "uuid", nullable: false),
                    NotificationDestinationId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_routing_rule_destinations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_routing_rule_destinations_notification_destinations_Notific~",
                        column: x => x.NotificationDestinationId,
                        principalTable: "notification_destinations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_routing_rule_destinations_routing_rules_RoutingRuleId",
                        column: x => x.RoutingRuleId,
                        principalTable: "routing_rules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_routing_rule_destinations_NotificationDestinationId",
                table: "routing_rule_destinations",
                column: "NotificationDestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_routing_rule_destinations_RoutingRuleId",
                table: "routing_rule_destinations",
                column: "RoutingRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_routing_rule_destinations_RoutingRuleId_NotificationDestina~",
                table: "routing_rule_destinations",
                columns: new[] { "RoutingRuleId", "NotificationDestinationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_routing_rules_Category",
                table: "routing_rules",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_routing_rules_IsActive",
                table: "routing_rules",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_routing_rules_rule_order",
                table: "routing_rules",
                column: "rule_order");

            migrationBuilder.CreateIndex(
                name: "IX_routing_rules_Severity",
                table: "routing_rules",
                column: "Severity");

            migrationBuilder.CreateIndex(
                name: "IX_routing_rules_Source",
                table: "routing_rules",
                column: "Source");

            migrationBuilder.CreateIndex(
                name: "IX_routing_rules_Type",
                table: "routing_rules",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "routing_rule_destinations");

            migrationBuilder.DropTable(
                name: "routing_rules");
        }
    }
}
