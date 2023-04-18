using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Achievements.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Adventurers",
                columns: table => new
                {
                    AdventurerId = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adventurers", x => x.AdventurerId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Lands",
                columns: table => new
                {
                    LandId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Explored = table.Column<bool>(type: "bit(1)", nullable: false),
                    AdventurerId = table.Column<uint>(type: "int unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lands", x => x.LandId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Dungeons",
                columns: table => new
                {
                    DungeonId = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Snapshot = table.Column<byte[]>(type: "longblob", nullable: false),
                    AdventurerId = table.Column<uint>(type: "int unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dungeons", x => x.DungeonId);
                    table.ForeignKey(
                        name: "FK_Dungeons_Adventurers_AdventurerId",
                        column: x => x.AdventurerId,
                        principalTable: "Adventurers",
                        principalColumn: "AdventurerId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Kills",
                columns: table => new
                {
                    KillId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Wcid = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    Count = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    AdventurerId = table.Column<uint>(type: "int unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kills", x => x.KillId);
                    table.ForeignKey(
                        name: "FK_Kills_Adventurers_AdventurerId",
                        column: x => x.AdventurerId,
                        principalTable: "Adventurers",
                        principalColumn: "AdventurerId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AdventurerLand",
                columns: table => new
                {
                    AdventurersAdventurerId = table.Column<uint>(type: "int unsigned", nullable: false),
                    LandsLandId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdventurerLand", x => new { x.AdventurersAdventurerId, x.LandsLandId });
                    table.ForeignKey(
                        name: "FK_AdventurerLand_Adventurers_AdventurersAdventurerId",
                        column: x => x.AdventurersAdventurerId,
                        principalTable: "Adventurers",
                        principalColumn: "AdventurerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdventurerLand_Lands_LandsLandId",
                        column: x => x.LandsLandId,
                        principalTable: "Lands",
                        principalColumn: "LandId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AdventurerLand_LandsLandId",
                table: "AdventurerLand",
                column: "LandsLandId");

            migrationBuilder.CreateIndex(
                name: "IX_Dungeons_AdventurerId",
                table: "Dungeons",
                column: "AdventurerId");

            migrationBuilder.CreateIndex(
                name: "IX_Kills_AdventurerId",
                table: "Kills",
                column: "AdventurerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdventurerLand");

            migrationBuilder.DropTable(
                name: "Dungeons");

            migrationBuilder.DropTable(
                name: "Kills");

            migrationBuilder.DropTable(
                name: "Lands");

            migrationBuilder.DropTable(
                name: "Adventurers");
        }
    }
}
