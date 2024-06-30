using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarBot.Data.Migrations
{
    public partial class Expeditions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "Expedition_1ID",
                table: "HustleCastleSettings",
                type: "bigint unsigned",
                nullable: true);

            migrationBuilder.AddColumn<ulong>(
                name: "Expedition_2ID",
                table: "HustleCastleSettings",
                type: "bigint unsigned",
                nullable: true);

            migrationBuilder.AddColumn<ulong>(
                name: "Expedition_3ID",
                table: "HustleCastleSettings",
                type: "bigint unsigned",
                nullable: true);

            migrationBuilder.AddColumn<ulong>(
                name: "Expedition_4ID",
                table: "HustleCastleSettings",
                type: "bigint unsigned",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Prep_Ending_Mins",
                table: "GuildChannelEvent",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HustleCastleSettings_Expedition_1ID",
                table: "HustleCastleSettings",
                column: "Expedition_1ID");

            migrationBuilder.CreateIndex(
                name: "IX_HustleCastleSettings_Expedition_2ID",
                table: "HustleCastleSettings",
                column: "Expedition_2ID");

            migrationBuilder.CreateIndex(
                name: "IX_HustleCastleSettings_Expedition_3ID",
                table: "HustleCastleSettings",
                column: "Expedition_3ID");

            migrationBuilder.CreateIndex(
                name: "IX_HustleCastleSettings_Expedition_4ID",
                table: "HustleCastleSettings",
                column: "Expedition_4ID");

            migrationBuilder.AddForeignKey(
                name: "FK_HustleCastleSettings_GuildChannelEvent_Expedition_1ID",
                table: "HustleCastleSettings",
                column: "Expedition_1ID",
                principalTable: "GuildChannelEvent",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_HustleCastleSettings_GuildChannelEvent_Expedition_2ID",
                table: "HustleCastleSettings",
                column: "Expedition_2ID",
                principalTable: "GuildChannelEvent",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_HustleCastleSettings_GuildChannelEvent_Expedition_3ID",
                table: "HustleCastleSettings",
                column: "Expedition_3ID",
                principalTable: "GuildChannelEvent",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_HustleCastleSettings_GuildChannelEvent_Expedition_4ID",
                table: "HustleCastleSettings",
                column: "Expedition_4ID",
                principalTable: "GuildChannelEvent",
                principalColumn: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HustleCastleSettings_GuildChannelEvent_Expedition_1ID",
                table: "HustleCastleSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_HustleCastleSettings_GuildChannelEvent_Expedition_2ID",
                table: "HustleCastleSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_HustleCastleSettings_GuildChannelEvent_Expedition_3ID",
                table: "HustleCastleSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_HustleCastleSettings_GuildChannelEvent_Expedition_4ID",
                table: "HustleCastleSettings");

            migrationBuilder.DropIndex(
                name: "IX_HustleCastleSettings_Expedition_1ID",
                table: "HustleCastleSettings");

            migrationBuilder.DropIndex(
                name: "IX_HustleCastleSettings_Expedition_2ID",
                table: "HustleCastleSettings");

            migrationBuilder.DropIndex(
                name: "IX_HustleCastleSettings_Expedition_3ID",
                table: "HustleCastleSettings");

            migrationBuilder.DropIndex(
                name: "IX_HustleCastleSettings_Expedition_4ID",
                table: "HustleCastleSettings");

            migrationBuilder.DropColumn(
                name: "Expedition_1ID",
                table: "HustleCastleSettings");

            migrationBuilder.DropColumn(
                name: "Expedition_2ID",
                table: "HustleCastleSettings");

            migrationBuilder.DropColumn(
                name: "Expedition_3ID",
                table: "HustleCastleSettings");

            migrationBuilder.DropColumn(
                name: "Expedition_4ID",
                table: "HustleCastleSettings");

            migrationBuilder.DropColumn(
                name: "Prep_Ending_Mins",
                table: "GuildChannelEvent");
        }
    }
}
