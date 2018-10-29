using Microsoft.EntityFrameworkCore.Migrations;

namespace WarBot.Storage.Migrations
{
    public partial class _3_AddPortalRemindar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PortalEnabled",
                table: "GuildNotificationsSettings",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PortalStartedMessage",
                table: "GuildNotificationsSettings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PortalEnabled",
                table: "GuildNotificationsSettings");

            migrationBuilder.DropColumn(
                name: "PortalStartedMessage",
                table: "GuildNotificationsSettings");
        }
    }
}
