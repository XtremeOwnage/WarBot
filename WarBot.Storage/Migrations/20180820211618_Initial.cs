using Microsoft.EntityFrameworkCore.Migrations;

namespace WarBot.Storage.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GuildNotificationsConfig",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WarPrepStarted = table.Column<bool>(nullable: false),
                    WarPrepAlmostOver = table.Column<bool>(nullable: false),
                    WarStarted = table.Column<bool>(nullable: false),
                    War1Enabled = table.Column<bool>(nullable: false),
                    War2Enabled = table.Column<bool>(nullable: false),
                    War3Enabled = table.Column<bool>(nullable: false),
                    War4Enabled = table.Column<bool>(nullable: false),
                    WarPrepStartedMessage = table.Column<string>(nullable: true),
                    WarPrepEndingMessage = table.Column<string>(nullable: true),
                    WarStartedMessage = table.Column<string>(nullable: true),
                    SendUpdateMessage = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildNotificationsConfig", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<ulong>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "GuildConfig",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Environment = table.Column<int>(nullable: false),
                    GuildNotificationSettingsId = table.Column<int>(nullable: true),
                    BotVersion = table.Column<string>(nullable: true),
                    Website = table.Column<string>(nullable: true),
                    Loot = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildConfig", x => x.ID);
                    table.ForeignKey(
                        name: "FK_GuildConfig_GuildNotificationsConfig_GuildNotificationSettingsId",
                        column: x => x.GuildNotificationSettingsId,
                        principalTable: "GuildNotificationsConfig",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GuildChannel",
                columns: table => new
                {
                    EntityId = table.Column<ulong>(nullable: true),
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    GuildConfigID = table.Column<int>(nullable: true),
                    ChannelType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildChannel", x => x.ID);
                    table.ForeignKey(
                        name: "FK_GuildChannel_GuildConfig_GuildConfigID",
                        column: x => x.GuildConfigID,
                        principalTable: "GuildConfig",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GuildRole",
                columns: table => new
                {
                    EntityId = table.Column<ulong>(nullable: true),
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Level = table.Column<int>(nullable: false),
                    GuildConfigID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildRole", x => x.ID);
                    table.ForeignKey(
                        name: "FK_GuildRole_GuildConfig_GuildConfigID",
                        column: x => x.GuildConfigID,
                        principalTable: "GuildConfig",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Guilds",
                columns: table => new
                {
                    GuildId = table.Column<ulong>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    ConfigId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guilds", x => x.GuildId);
                    table.ForeignKey(
                        name: "FK_Guilds_GuildConfig_ConfigId",
                        column: x => x.ConfigId,
                        principalTable: "GuildConfig",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GuildChannel_GuildConfigID",
                table: "GuildChannel",
                column: "GuildConfigID");

            migrationBuilder.CreateIndex(
                name: "IX_GuildConfig_GuildNotificationSettingsId",
                table: "GuildConfig",
                column: "GuildNotificationSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildRole_GuildConfigID",
                table: "GuildRole",
                column: "GuildConfigID");

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_ConfigId",
                table: "Guilds",
                column: "ConfigId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuildChannel");

            migrationBuilder.DropTable(
                name: "GuildRole");

            migrationBuilder.DropTable(
                name: "Guilds");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "GuildConfig");

            migrationBuilder.DropTable(
                name: "GuildNotificationsConfig");
        }
    }
}
