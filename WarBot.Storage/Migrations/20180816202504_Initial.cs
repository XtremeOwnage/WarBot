using Microsoft.EntityFrameworkCore.Migrations;

namespace WarBot.Storage.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiscordEntity",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EntityId = table.Column<ulong>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordEntity", x => x.ID);
                });

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
                    Loot = table.Column<string>(nullable: true),
                    ch_war_id = table.Column<int>(nullable: true),
                    ch_welcome_id = table.Column<int>(nullable: true),
                    ch_news_id = table.Column<int>(nullable: true),
                    ch_officers_id = table.Column<int>(nullable: true),
                    role_admin_id = table.Column<int>(nullable: true),
                    role_leader_id = table.Column<int>(nullable: true),
                    role_officer_id = table.Column<int>(nullable: true),
                    role_member_id = table.Column<int>(nullable: true)
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
                    table.ForeignKey(
                        name: "FK_GuildConfig_DiscordEntity_ch_news_id",
                        column: x => x.ch_news_id,
                        principalTable: "DiscordEntity",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GuildConfig_DiscordEntity_ch_officers_id",
                        column: x => x.ch_officers_id,
                        principalTable: "DiscordEntity",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GuildConfig_DiscordEntity_ch_war_id",
                        column: x => x.ch_war_id,
                        principalTable: "DiscordEntity",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GuildConfig_DiscordEntity_ch_welcome_id",
                        column: x => x.ch_welcome_id,
                        principalTable: "DiscordEntity",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GuildConfig_DiscordEntity_role_admin_id",
                        column: x => x.role_admin_id,
                        principalTable: "DiscordEntity",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GuildConfig_DiscordEntity_role_leader_id",
                        column: x => x.role_leader_id,
                        principalTable: "DiscordEntity",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GuildConfig_DiscordEntity_role_member_id",
                        column: x => x.role_member_id,
                        principalTable: "DiscordEntity",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GuildConfig_DiscordEntity_role_officer_id",
                        column: x => x.role_officer_id,
                        principalTable: "DiscordEntity",
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
                name: "IX_GuildConfig_GuildNotificationSettingsId",
                table: "GuildConfig",
                column: "GuildNotificationSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildConfig_ch_news_id",
                table: "GuildConfig",
                column: "ch_news_id");

            migrationBuilder.CreateIndex(
                name: "IX_GuildConfig_ch_officers_id",
                table: "GuildConfig",
                column: "ch_officers_id");

            migrationBuilder.CreateIndex(
                name: "IX_GuildConfig_ch_war_id",
                table: "GuildConfig",
                column: "ch_war_id");

            migrationBuilder.CreateIndex(
                name: "IX_GuildConfig_ch_welcome_id",
                table: "GuildConfig",
                column: "ch_welcome_id");

            migrationBuilder.CreateIndex(
                name: "IX_GuildConfig_role_admin_id",
                table: "GuildConfig",
                column: "role_admin_id");

            migrationBuilder.CreateIndex(
                name: "IX_GuildConfig_role_leader_id",
                table: "GuildConfig",
                column: "role_leader_id");

            migrationBuilder.CreateIndex(
                name: "IX_GuildConfig_role_member_id",
                table: "GuildConfig",
                column: "role_member_id");

            migrationBuilder.CreateIndex(
                name: "IX_GuildConfig_role_officer_id",
                table: "GuildConfig",
                column: "role_officer_id");

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_ConfigId",
                table: "Guilds",
                column: "ConfigId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Guilds");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "GuildConfig");

            migrationBuilder.DropTable(
                name: "GuildNotificationsConfig");

            migrationBuilder.DropTable(
                name: "DiscordEntity");
        }
    }
}
