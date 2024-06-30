using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarBot.Data.Migrations
{
    public partial class DiscordEvents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TimeZone",
                table: "GuildSettings",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "CreateEvent",
                table: "GuildChannelEvent",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "EventDescription",
                table: "GuildChannelEvent",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "EventTitle",
                table: "GuildChannelEvent",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeZone",
                table: "GuildSettings");

            migrationBuilder.DropColumn(
                name: "CreateEvent",
                table: "GuildChannelEvent");

            migrationBuilder.DropColumn(
                name: "EventDescription",
                table: "GuildChannelEvent");

            migrationBuilder.DropColumn(
                name: "EventTitle",
                table: "GuildChannelEvent");
        }
    }
}
