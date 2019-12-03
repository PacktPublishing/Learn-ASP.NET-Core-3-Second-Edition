using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TicTacToe.Migrations
{
    public partial class InitialDbSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    FirstName = table.Column<string>(nullable: false),
                    LastName = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: false),
                    IsEmailConfirmed = table.Column<bool>(nullable: false),
                    EmailConfirmationDate = table.Column<DateTime>(nullable: true),
                    Score = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameInvitationModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    EmailTo = table.Column<string>(nullable: true),
                    InvitedBy = table.Column<string>(nullable: true),
                    InvitedByUserId = table.Column<Guid>(nullable: false),
                    IsConfirmed = table.Column<bool>(nullable: false),
                    ConfirmationDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameInvitationModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameInvitationModel_UserModel_InvitedByUserId",
                        column: x => x.InvitedByUserId,
                        principalTable: "UserModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameSessionModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId1 = table.Column<Guid>(nullable: false),
                    UserId2 = table.Column<Guid>(nullable: false),
                    User2Id = table.Column<Guid>(nullable: true),
                    WinnerId = table.Column<Guid>(nullable: false),
                    ActiveUserId = table.Column<Guid>(nullable: false),
                    TurnFinished = table.Column<bool>(nullable: false),
                    TurnNumber = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSessionModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameSessionModel_UserModel_User2Id",
                        column: x => x.User2Id,
                        principalTable: "UserModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GameSessionModel_UserModel_UserId1",
                        column: x => x.UserId1,
                        principalTable: "UserModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TurnModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    X = table.Column<int>(nullable: false),
                    Y = table.Column<int>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    IconNumber = table.Column<string>(nullable: true),
                    GameSessionModelId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TurnModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TurnModel_GameSessionModel_GameSessionModelId",
                        column: x => x.GameSessionModelId,
                        principalTable: "GameSessionModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TurnModel_UserModel_UserId",
                        column: x => x.UserId,
                        principalTable: "UserModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameInvitationModel_InvitedByUserId",
                table: "GameInvitationModel",
                column: "InvitedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_GameSessionModel_User2Id",
                table: "GameSessionModel",
                column: "User2Id");

            migrationBuilder.CreateIndex(
                name: "IX_GameSessionModel_UserId1",
                table: "GameSessionModel",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_TurnModel_GameSessionModelId",
                table: "TurnModel",
                column: "GameSessionModelId");

            migrationBuilder.CreateIndex(
                name: "IX_TurnModel_UserId",
                table: "TurnModel",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameInvitationModel");

            migrationBuilder.DropTable(
                name: "TurnModel");

            migrationBuilder.DropTable(
                name: "GameSessionModel");

            migrationBuilder.DropTable(
                name: "UserModel");
        }
    }
}
