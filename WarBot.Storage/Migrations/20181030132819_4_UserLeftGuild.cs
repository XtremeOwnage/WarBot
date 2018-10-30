using Microsoft.EntityFrameworkCore.Migrations;

namespace WarBot.Storage.Migrations
{
    public partial class _4_UserLeftGuild : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "User_Left_Guild",
                table: "GuildNotificationsSettings",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "User_Left_Guild",
                table: "GuildNotificationsSettings");
        }
    }
}
