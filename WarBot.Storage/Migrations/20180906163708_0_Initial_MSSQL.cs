using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WarBot.Storage.Migrations
{
    public partial class _0_Initial_MSSQL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GuildNotificationsSettings",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false),
                    WarPrepStarted = table.Column<bool>(nullable: false),
                    WarPrepEnding = table.Column<bool>(nullable: false),
                    WarStarted = table.Column<bool>(nullable: false),
                    War1Enabled = table.Column<bool>(nullable: false),
                    War2Enabled = table.Column<bool>(nullable: false),
                    War3Enabled = table.Column<bool>(nullable: false),
                    War4Enabled = table.Column<bool>(nullable: false),
                    WarPrepStartedMessage = table.Column<string>(nullable: true),
                    WarPrepEndingMessage = table.Column<string>(nullable: true),
                    WarStartedMessage = table.Column<string>(nullable: true),
                    SendUpdateMessage = table.Column<bool>(nullable: false),
                    GreetingMessage = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildNotificationsSettings", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EntityId = table.Column<decimal>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    LastOnline = table.Column<DateTimeOffset>(nullable: true),
                    LastActivity = table.Column<DateTimeOffset>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Guilds",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EntityId = table.Column<decimal>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    WarBOT_NickName = table.Column<string>(nullable: true),
                    WarBOT_Prefix = table.Column<string>(nullable: true),
                    Environment = table.Column<int>(nullable: false),
                    NotificationSettingsID = table.Column<long>(nullable: true),
                    BotVersion = table.Column<string>(nullable: true),
                    Website = table.Column<string>(nullable: true),
                    Loot = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guilds", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Guilds_GuildNotificationsSettings_NotificationSettingsID",
                        column: x => x.NotificationSettingsID,
                        principalTable: "GuildNotificationsSettings",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GuildChannel",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EntityId = table.Column<decimal>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    GuildID = table.Column<int>(nullable: true),
                    ChannelType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildChannel", x => x.ID);
                    table.ForeignKey(
                        name: "FK_GuildChannel_Guilds_GuildID",
                        column: x => x.GuildID,
                        principalTable: "Guilds",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GuildRole",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EntityId = table.Column<decimal>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Level = table.Column<int>(nullable: false),
                    GuildID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildRole", x => x.ID);
                    table.ForeignKey(
                        name: "FK_GuildRole_Guilds_GuildID",
                        column: x => x.GuildID,
                        principalTable: "Guilds",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HustleGuild",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    DiscordGuildId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HustleGuild", x => x.ID);
                    table.ForeignKey(
                        name: "FK_HustleGuild_Guilds_DiscordGuildId",
                        column: x => x.DiscordGuildId,
                        principalTable: "Guilds",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HustleUser",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CharacterName = table.Column<string>(maxLength: 20, nullable: false),
                    ThroneRoomLevel = table.Column<byte>(nullable: false),
                    SquadPower = table.Column<long>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    IsActiveCharacter = table.Column<bool>(nullable: false),
                    JoinDate = table.Column<DateTimeOffset>(nullable: false),
                    HustleGuildID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HustleUser", x => x.ID);
                    table.ForeignKey(
                        name: "FK_HustleUser_HustleGuild_HustleGuildID",
                        column: x => x.HustleGuildID,
                        principalTable: "HustleGuild",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HustleUser_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HustleGuildSeason",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateEnd = table.Column<DateTime>(nullable: false),
                    MinimumGuildGlory = table.Column<long>(nullable: false),
                    MinimumGameGlory = table.Column<long>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    HustleGuildID = table.Column<int>(nullable: true),
                    HustleUserID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HustleGuildSeason", x => x.ID);
                    table.ForeignKey(
                        name: "FK_HustleGuildSeason_HustleGuild_HustleGuildID",
                        column: x => x.HustleGuildID,
                        principalTable: "HustleGuild",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HustleGuildSeason_HustleUser_HustleUserID",
                        column: x => x.HustleUserID,
                        principalTable: "HustleUser",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HustleUserSeason",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SeasonID = table.Column<int>(nullable: true),
                    UserID = table.Column<int>(nullable: true),
                    Glory = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HustleUserSeason", x => x.ID);
                    table.ForeignKey(
                        name: "FK_HustleUserSeason_HustleGuildSeason_SeasonID",
                        column: x => x.SeasonID,
                        principalTable: "HustleGuildSeason",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HustleUserSeason_HustleUser_UserID",
                        column: x => x.UserID,
                        principalTable: "HustleUser",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LootItem",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    WinningUserID = table.Column<int>(nullable: true),
                    HustleGuildSeasonID = table.Column<int>(nullable: true),
                    HustleUserSeasonID = table.Column<int>(nullable: true),
                    HustleUserSeasonID1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LootItem", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LootItem_HustleGuildSeason_HustleGuildSeasonID",
                        column: x => x.HustleGuildSeasonID,
                        principalTable: "HustleGuildSeason",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LootItem_HustleUserSeason_HustleUserSeasonID",
                        column: x => x.HustleUserSeasonID,
                        principalTable: "HustleUserSeason",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LootItem_HustleUserSeason_HustleUserSeasonID1",
                        column: x => x.HustleUserSeasonID1,
                        principalTable: "HustleUserSeason",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LootItem_HustleUser_WinningUserID",
                        column: x => x.WinningUserID,
                        principalTable: "HustleUser",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GuildChannel_GuildID",
                table: "GuildChannel",
                column: "GuildID");

            migrationBuilder.CreateIndex(
                name: "IX_GuildRole_GuildID",
                table: "GuildRole",
                column: "GuildID");

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_NotificationSettingsID",
                table: "Guilds",
                column: "NotificationSettingsID");

            migrationBuilder.CreateIndex(
                name: "IX_HustleGuild_DiscordGuildId",
                table: "HustleGuild",
                column: "DiscordGuildId",
                unique: true,
                filter: "[DiscordGuildId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HustleGuildSeason_HustleGuildID",
                table: "HustleGuildSeason",
                column: "HustleGuildID");

            migrationBuilder.CreateIndex(
                name: "IX_HustleGuildSeason_HustleUserID",
                table: "HustleGuildSeason",
                column: "HustleUserID");

            migrationBuilder.CreateIndex(
                name: "IX_HustleUser_HustleGuildID",
                table: "HustleUser",
                column: "HustleGuildID");

            migrationBuilder.CreateIndex(
                name: "IX_HustleUser_UserID",
                table: "HustleUser",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_HustleUserSeason_SeasonID",
                table: "HustleUserSeason",
                column: "SeasonID");

            migrationBuilder.CreateIndex(
                name: "IX_HustleUserSeason_UserID",
                table: "HustleUserSeason",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_LootItem_HustleGuildSeasonID",
                table: "LootItem",
                column: "HustleGuildSeasonID");

            migrationBuilder.CreateIndex(
                name: "IX_LootItem_HustleUserSeasonID",
                table: "LootItem",
                column: "HustleUserSeasonID");

            migrationBuilder.CreateIndex(
                name: "IX_LootItem_HustleUserSeasonID1",
                table: "LootItem",
                column: "HustleUserSeasonID1");

            migrationBuilder.CreateIndex(
                name: "IX_LootItem_WinningUserID",
                table: "LootItem",
                column: "WinningUserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuildChannel");

            migrationBuilder.DropTable(
                name: "GuildRole");

            migrationBuilder.DropTable(
                name: "LootItem");

            migrationBuilder.DropTable(
                name: "HustleUserSeason");

            migrationBuilder.DropTable(
                name: "HustleGuildSeason");

            migrationBuilder.DropTable(
                name: "HustleUser");

            migrationBuilder.DropTable(
                name: "HustleGuild");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Guilds");

            migrationBuilder.DropTable(
                name: "GuildNotificationsSettings");
        }
    }
}
