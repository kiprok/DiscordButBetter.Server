using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscordButBetter.Server.Migrations
{
    /// <inheritdoc />
    public partial class SessionMoreData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "expiration",
                table: "Sessions");

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "Sessions",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "UserAgent",
                table: "Sessions",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "UserAgent",
                table: "Sessions");

            migrationBuilder.AddColumn<DateTime>(
                name: "expiration",
                table: "Sessions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
