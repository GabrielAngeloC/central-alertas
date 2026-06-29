using System;
using CentralAlertas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CentralAlertas.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(CentralAlertasDbContext))]
    [Migration("20260628140000_AddSoftDelete")]
    public partial class AddSoftDelete : Migration
    {
        private static readonly string[] Tables =
        {
            "routing_rules",
            "notification_destinations",
            "sources",
            "allowed_origins"
        };

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            foreach (var table in Tables)
            {
                migrationBuilder.AddColumn<bool>(
                    name: "IsDeleted",
                    table: table,
                    type: "boolean",
                    nullable: false,
                    defaultValue: false);

                migrationBuilder.AddColumn<DateTime>(
                    name: "DeletedAt",
                    table: table,
                    type: "timestamp with time zone",
                    nullable: true);
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            foreach (var table in Tables)
            {
                migrationBuilder.DropColumn(name: "IsDeleted", table: table);
                migrationBuilder.DropColumn(name: "DeletedAt", table: table);
            }
        }
    }
}
