using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscordButBetter.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddedFriends : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserModelUserModel",
                columns: table => new
                {
                    FriendRequestsId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FriendsId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserModelUserModel", x => new { x.FriendRequestsId, x.FriendsId });
                    table.ForeignKey(
                        name: "FK_UserModelUserModel_Users_FriendRequestsId",
                        column: x => x.FriendRequestsId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserModelUserModel_Users_FriendsId",
                        column: x => x.FriendsId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_UserModelUserModel_FriendsId",
                table: "UserModelUserModel",
                column: "FriendsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserModelUserModel");
        }
    }
}
