using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CentralAlertas.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSources : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    ExpectedIntervalMinutes = table.Column<int>(type: "integer", nullable: false),
                    LastReceivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sources", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sources_IsActive",
                table: "sources",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_sources_LastReceivedAt",
                table: "sources",
                column: "LastReceivedAt");

            migrationBuilder.CreateIndex(
                name: "IX_sources_Name",
                table: "sources",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sources");
        }
    }
}
