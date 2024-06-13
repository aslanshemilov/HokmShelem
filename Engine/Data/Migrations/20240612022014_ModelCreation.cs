using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Engine.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModelCreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Card",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Card1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Card2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Card3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Card4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Card5 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Card6 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Card7 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Card8 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Card9 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Card10 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Card11 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Card12 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Card13 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Card", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Lobby",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lobby", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Room",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GameType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TargetScore = table.Column<int>(type: "int", nullable: false),
                    HostName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Blue1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Red1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Blue2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Red2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Blue1Ready = table.Column<bool>(type: "bit", nullable: false),
                    Red1Ready = table.Column<bool>(type: "bit", nullable: false),
                    Blue2Ready = table.Column<bool>(type: "bit", nullable: false),
                    Red2Ready = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Room", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Game",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GameType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TargetScore = table.Column<int>(type: "int", nullable: false),
                    Blue1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Red1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Blue2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Red2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GS = table.Column<int>(type: "int", nullable: false),
                    HakemIndex = table.Column<int>(type: "int", nullable: false),
                    NthCardIsBeingPlayed = table.Column<int>(type: "int", nullable: false),
                    HokmSuit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoundSuit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Blue1Card = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Red1Card = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Blue2Card = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Red2Card = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RedRoundScore = table.Column<int>(type: "int", nullable: false),
                    BlueRoundScore = table.Column<int>(type: "int", nullable: false),
                    RedTotalScore = table.Column<int>(type: "int", nullable: false),
                    BlueTotalScore = table.Column<int>(type: "int", nullable: false),
                    WhosTurnIndex = table.Column<int>(type: "int", nullable: false),
                    RoundStartsByIndex = table.Column<int>(type: "int", nullable: false),
                    RoundTargetScore = table.Column<int>(type: "int", nullable: false),
                    ClaimStartsByIndex = table.Column<int>(type: "int", nullable: false),
                    Blue1Claimed = table.Column<int>(type: "int", nullable: false),
                    Red1Claimed = table.Column<int>(type: "int", nullable: false),
                    Blue2Claimed = table.Column<int>(type: "int", nullable: false),
                    Red2Claimed = table.Column<int>(type: "int", nullable: false),
                    Blue1Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Red1Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Blue2Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Red2Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Blue1CardsName = table.Column<string>(type: "nvarchar(20)", nullable: true),
                    Red1CardsName = table.Column<string>(type: "nvarchar(20)", nullable: true),
                    Blue2CardsName = table.Column<string>(type: "nvarchar(20)", nullable: true),
                    Red2CardsName = table.Column<string>(type: "nvarchar(20)", nullable: true),
                    HakemCardsName = table.Column<string>(type: "nvarchar(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Game", x => x.Name);
                    table.ForeignKey(
                        name: "FK_Game_Card_Blue1CardsName",
                        column: x => x.Blue1CardsName,
                        principalTable: "Card",
                        principalColumn: "Name");
                    table.ForeignKey(
                        name: "FK_Game_Card_Blue2CardsName",
                        column: x => x.Blue2CardsName,
                        principalTable: "Card",
                        principalColumn: "Name");
                    table.ForeignKey(
                        name: "FK_Game_Card_HakemCardsName",
                        column: x => x.HakemCardsName,
                        principalTable: "Card",
                        principalColumn: "Name");
                    table.ForeignKey(
                        name: "FK_Game_Card_Red1CardsName",
                        column: x => x.Red1CardsName,
                        principalTable: "Card",
                        principalColumn: "Name");
                    table.ForeignKey(
                        name: "FK_Game_Card_Red2CardsName",
                        column: x => x.Red2CardsName,
                        principalTable: "Card",
                        principalColumn: "Name");
                });

            migrationBuilder.CreateTable(
                name: "Player",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ConnectionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Badge = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rate = table.Column<int>(type: "int", nullable: false),
                    HokmScore = table.Column<int>(type: "int", nullable: false),
                    ShelemScore = table.Column<int>(type: "int", nullable: false),
                    GamesWon = table.Column<int>(type: "int", nullable: false),
                    GamesLost = table.Column<int>(type: "int", nullable: false),
                    GamesLeft = table.Column<int>(type: "int", nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LobbyName = table.Column<string>(type: "nvarchar(20)", nullable: true),
                    RoomName = table.Column<string>(type: "nvarchar(20)", nullable: true),
                    GameName = table.Column<string>(type: "nvarchar(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Player", x => x.Name);
                    table.ForeignKey(
                        name: "FK_Player_Game_GameName",
                        column: x => x.GameName,
                        principalTable: "Game",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Player_Lobby_LobbyName",
                        column: x => x.LobbyName,
                        principalTable: "Lobby",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Player_Room_RoomName",
                        column: x => x.RoomName,
                        principalTable: "Room",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConnectionTracker",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CurrentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OldId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlayerName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConnectionTracker", x => x.Name);
                    table.ForeignKey(
                        name: "FK_ConnectionTracker_Player_Name",
                        column: x => x.Name,
                        principalTable: "Player",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Game_Blue1CardsName",
                table: "Game",
                column: "Blue1CardsName");

            migrationBuilder.CreateIndex(
                name: "IX_Game_Blue2CardsName",
                table: "Game",
                column: "Blue2CardsName");

            migrationBuilder.CreateIndex(
                name: "IX_Game_HakemCardsName",
                table: "Game",
                column: "HakemCardsName");

            migrationBuilder.CreateIndex(
                name: "IX_Game_Red1CardsName",
                table: "Game",
                column: "Red1CardsName");

            migrationBuilder.CreateIndex(
                name: "IX_Game_Red2CardsName",
                table: "Game",
                column: "Red2CardsName");

            migrationBuilder.CreateIndex(
                name: "IX_Player_GameName",
                table: "Player",
                column: "GameName");

            migrationBuilder.CreateIndex(
                name: "IX_Player_LobbyName",
                table: "Player",
                column: "LobbyName");

            migrationBuilder.CreateIndex(
                name: "IX_Player_RoomName",
                table: "Player",
                column: "RoomName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConnectionTracker");

            migrationBuilder.DropTable(
                name: "Player");

            migrationBuilder.DropTable(
                name: "Game");

            migrationBuilder.DropTable(
                name: "Lobby");

            migrationBuilder.DropTable(
                name: "Room");

            migrationBuilder.DropTable(
                name: "Card");
        }
    }
}
