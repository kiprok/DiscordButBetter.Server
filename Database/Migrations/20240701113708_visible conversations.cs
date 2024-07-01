using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscordButBetter.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class visibleconversations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConversationModelUserModel");

            migrationBuilder.CreateTable(
                name: "conversationParticipants",
                columns: table => new
                {
                    ConversationId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_conversationParticipants", x => new { x.ConversationId, x.UserId });
                    table.ForeignKey(
                        name: "FK_conversationParticipants_Conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_conversationParticipants_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "conversationParticipantsVisible",
                columns: table => new
                {
                    ConversationId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_conversationParticipantsVisible", x => new { x.ConversationId, x.UserId });
                    table.ForeignKey(
                        name: "FK_conversationParticipantsVisible_Conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_conversationParticipantsVisible_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_conversationParticipants_UserId",
                table: "conversationParticipants",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_conversationParticipantsVisible_UserId",
                table: "conversationParticipantsVisible",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "conversationParticipants");

            migrationBuilder.DropTable(
                name: "conversationParticipantsVisible");

            migrationBuilder.CreateTable(
                name: "ConversationModelUserModel",
                columns: table => new
                {
                    ConversationsId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ParticipantsId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversationModelUserModel", x => new { x.ConversationsId, x.ParticipantsId });
                    table.ForeignKey(
                        name: "FK_ConversationModelUserModel_Conversations_ConversationsId",
                        column: x => x.ConversationsId,
                        principalTable: "Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConversationModelUserModel_Users_ParticipantsId",
                        column: x => x.ParticipantsId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationModelUserModel_ParticipantsId",
                table: "ConversationModelUserModel",
                column: "ParticipantsId");
        }
    }
}
