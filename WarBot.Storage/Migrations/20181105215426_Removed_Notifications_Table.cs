using Microsoft.EntityFrameworkCore.Migrations;

namespace WarBot.Storage.Migrations
{
    public partial class Removed_Notifications_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //SQL Lite does not support dropping keys or columns.
            //So, I guess these values will just float around the schema for a while........ /Shrugs.
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Guilds_GuildNotificationsSettings_NotificationSettingsID",
            //    table: "Guilds");

            //migrationBuilder.DropTable(
            //    name: "GuildNotificationsSettings");

            //migrationBuilder.DropIndex(
            //    name: "IX_Guilds_NotificationSettingsID",
            //    table: "Guilds");

            //migrationBuilder.DropColumn(
            //    name: "NotificationSettingsID",
            //    table: "Guilds");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NotificationSettingsID",
                table: "Guilds",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GuildNotificationsSettings",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GreetingMessage = table.Column<string>(nullable: true),
                    PortalEnabled = table.Column<bool>(nullable: false),
                    PortalStartedMessage = table.Column<string>(nullable: true),
                    SendUpdateMessage = table.Column<bool>(nullable: false),
                    User_Left_Guild = table.Column<bool>(nullable: false),
                    War1Enabled = table.Column<bool>(nullable: false),
                    War2Enabled = table.Column<bool>(nullable: false),
                    War3Enabled = table.Column<bool>(nullable: false),
                    War4Enabled = table.Column<bool>(nullable: false),
                    WarPrepEnding = table.Column<bool>(nullable: false),
                    WarPrepEndingMessage = table.Column<string>(nullable: true),
                    WarPrepStarted = table.Column<bool>(nullable: false),
                    WarPrepStartedMessage = table.Column<string>(nullable: true),
                    WarStarted = table.Column<bool>(nullable: false),
                    WarStartedMessage = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildNotificationsSettings", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_NotificationSettingsID",
                table: "Guilds",
                column: "NotificationSettingsID");

            migrationBuilder.AddForeignKey(
                name: "FK_Guilds_GuildNotificationsSettings_NotificationSettingsID",
                table: "Guilds",
                column: "NotificationSettingsID",
                principalTable: "GuildNotificationsSettings",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
