using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MythicNights.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GuildConfigs",
                columns: table => new
                {
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChannelId = table.Column<ulong>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildConfigs", x => x.GuildId);
                });

            migrationBuilder.CreateTable(
                name: "MythicNights",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EventTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    Attending = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MythicNights", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    DiscordUserId = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DiscordUsername = table.Column<string>(type: "TEXT", nullable: true),
                    Nickname = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.DiscordUserId);
                });

            migrationBuilder.CreateTable(
                name: "Toons",
                columns: table => new
                {
                    FullName = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Realm = table.Column<string>(type: "TEXT", nullable: true),
                    iLvl = table.Column<double>(type: "REAL", nullable: false),
                    RaiderIO = table.Column<double>(type: "REAL", nullable: false),
                    PreferedRole = table.Column<int>(type: "INTEGER", nullable: false),
                    Offspec = table.Column<int>(type: "INTEGER", nullable: true),
                    IsPrefered = table.Column<bool>(type: "INTEGER", nullable: true),
                    PlayerDiscordUserId = table.Column<ulong>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Toons", x => x.FullName);
                    table.ForeignKey(
                        name: "FK_Toons_Players_PlayerDiscordUserId",
                        column: x => x.PlayerDiscordUserId,
                        principalTable: "Players",
                        principalColumn: "DiscordUserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Toons_PlayerDiscordUserId",
                table: "Toons",
                column: "PlayerDiscordUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuildConfigs");

            migrationBuilder.DropTable(
                name: "MythicNights");

            migrationBuilder.DropTable(
                name: "Toons");

            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}
