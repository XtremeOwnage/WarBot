using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarBot.Data.Migrations
{
    public partial class _1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "GuildChannel",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DiscordID = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                    DiscordName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildChannel", x => x.ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "GuildRole",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DiscordID = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                    DiscordName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildRole", x => x.ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "GuildChannelEvent",
                columns: table => new
                {
                    ID = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Enabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ChannelID = table.Column<long>(type: "bigint", nullable: false),
                    Message = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Discriminator = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Prep_Started_Message = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Prep_Ending_Message = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Event_Started_Message = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Event_Finished_Message = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildChannelEvent", x => x.ID);
                    table.ForeignKey(
                        name: "FK_GuildChannelEvent_GuildChannel_ChannelID",
                        column: x => x.ChannelID,
                        principalTable: "GuildChannel",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "GuildRoles",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GuestID = table.Column<long>(type: "bigint", nullable: false),
                    MemberID = table.Column<long>(type: "bigint", nullable: false),
                    SuperMemberID = table.Column<long>(type: "bigint", nullable: false),
                    OfficerID = table.Column<long>(type: "bigint", nullable: false),
                    LeaderID = table.Column<long>(type: "bigint", nullable: false),
                    ServerAdminID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildRoles", x => x.ID);
                    table.ForeignKey(
                        name: "FK_GuildRoles_GuildRole_GuestID",
                        column: x => x.GuestID,
                        principalTable: "GuildRole",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuildRoles_GuildRole_LeaderID",
                        column: x => x.LeaderID,
                        principalTable: "GuildRole",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuildRoles_GuildRole_MemberID",
                        column: x => x.MemberID,
                        principalTable: "GuildRole",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuildRoles_GuildRole_OfficerID",
                        column: x => x.OfficerID,
                        principalTable: "GuildRole",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuildRoles_GuildRole_ServerAdminID",
                        column: x => x.ServerAdminID,
                        principalTable: "GuildRole",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuildRoles_GuildRole_SuperMemberID",
                        column: x => x.SuperMemberID,
                        principalTable: "GuildRole",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "HustleCastleSettings",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LootMessage = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PortalID = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    War_1ID = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    War_2ID = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    War_3ID = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    War_4ID = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HustleCastleSettings", x => x.ID);
                    table.ForeignKey(
                        name: "FK_HustleCastleSettings_GuildChannelEvent_PortalID",
                        column: x => x.PortalID,
                        principalTable: "GuildChannelEvent",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HustleCastleSettings_GuildChannelEvent_War_1ID",
                        column: x => x.War_1ID,
                        principalTable: "GuildChannelEvent",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HustleCastleSettings_GuildChannelEvent_War_2ID",
                        column: x => x.War_2ID,
                        principalTable: "GuildChannelEvent",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HustleCastleSettings_GuildChannelEvent_War_3ID",
                        column: x => x.War_3ID,
                        principalTable: "GuildChannelEvent",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HustleCastleSettings_GuildChannelEvent_War_4ID",
                        column: x => x.War_4ID,
                        principalTable: "GuildChannelEvent",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "GuildSettings",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DiscordID = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    DiscordName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BotVersion = table.Column<double>(type: "double", nullable: false),
                    BotPrefix = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Website = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Channel_AdminsID = table.Column<long>(type: "bigint", nullable: false),
                    Event_UpdatesID = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    Event_UserJoinID = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    Event_UserLeftID = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    HustleCastleSettingsID = table.Column<long>(type: "bigint", nullable: false),
                    RolesID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildSettings", x => x.ID);
                    table.ForeignKey(
                        name: "FK_GuildSettings_GuildChannel_Channel_AdminsID",
                        column: x => x.Channel_AdminsID,
                        principalTable: "GuildChannel",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuildSettings_GuildChannelEvent_Event_UpdatesID",
                        column: x => x.Event_UpdatesID,
                        principalTable: "GuildChannelEvent",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuildSettings_GuildChannelEvent_Event_UserJoinID",
                        column: x => x.Event_UserJoinID,
                        principalTable: "GuildChannelEvent",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuildSettings_GuildChannelEvent_Event_UserLeftID",
                        column: x => x.Event_UserLeftID,
                        principalTable: "GuildChannelEvent",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuildSettings_GuildRoles_RolesID",
                        column: x => x.RolesID,
                        principalTable: "GuildRoles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuildSettings_HustleCastleSettings_HustleCastleSettingsID",
                        column: x => x.HustleCastleSettingsID,
                        principalTable: "HustleCastleSettings",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_GuildChannelEvent_ChannelID",
                table: "GuildChannelEvent",
                column: "ChannelID");

            migrationBuilder.CreateIndex(
                name: "IX_GuildRoles_GuestID",
                table: "GuildRoles",
                column: "GuestID");

            migrationBuilder.CreateIndex(
                name: "IX_GuildRoles_LeaderID",
                table: "GuildRoles",
                column: "LeaderID");

            migrationBuilder.CreateIndex(
                name: "IX_GuildRoles_MemberID",
                table: "GuildRoles",
                column: "MemberID");

            migrationBuilder.CreateIndex(
                name: "IX_GuildRoles_OfficerID",
                table: "GuildRoles",
                column: "OfficerID");

            migrationBuilder.CreateIndex(
                name: "IX_GuildRoles_ServerAdminID",
                table: "GuildRoles",
                column: "ServerAdminID");

            migrationBuilder.CreateIndex(
                name: "IX_GuildRoles_SuperMemberID",
                table: "GuildRoles",
                column: "SuperMemberID");

            migrationBuilder.CreateIndex(
                name: "IX_GuildSettings_Channel_AdminsID",
                table: "GuildSettings",
                column: "Channel_AdminsID");

            migrationBuilder.CreateIndex(
                name: "IX_GuildSettings_Event_UpdatesID",
                table: "GuildSettings",
                column: "Event_UpdatesID");

            migrationBuilder.CreateIndex(
                name: "IX_GuildSettings_Event_UserJoinID",
                table: "GuildSettings",
                column: "Event_UserJoinID");

            migrationBuilder.CreateIndex(
                name: "IX_GuildSettings_Event_UserLeftID",
                table: "GuildSettings",
                column: "Event_UserLeftID");

            migrationBuilder.CreateIndex(
                name: "IX_GuildSettings_HustleCastleSettingsID",
                table: "GuildSettings",
                column: "HustleCastleSettingsID");

            migrationBuilder.CreateIndex(
                name: "IX_GuildSettings_RolesID",
                table: "GuildSettings",
                column: "RolesID");

            migrationBuilder.CreateIndex(
                name: "IX_HustleCastleSettings_PortalID",
                table: "HustleCastleSettings",
                column: "PortalID");

            migrationBuilder.CreateIndex(
                name: "IX_HustleCastleSettings_War_1ID",
                table: "HustleCastleSettings",
                column: "War_1ID");

            migrationBuilder.CreateIndex(
                name: "IX_HustleCastleSettings_War_2ID",
                table: "HustleCastleSettings",
                column: "War_2ID");

            migrationBuilder.CreateIndex(
                name: "IX_HustleCastleSettings_War_3ID",
                table: "HustleCastleSettings",
                column: "War_3ID");

            migrationBuilder.CreateIndex(
                name: "IX_HustleCastleSettings_War_4ID",
                table: "HustleCastleSettings",
                column: "War_4ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuildSettings");

            migrationBuilder.DropTable(
                name: "GuildRoles");

            migrationBuilder.DropTable(
                name: "HustleCastleSettings");

            migrationBuilder.DropTable(
                name: "GuildRole");

            migrationBuilder.DropTable(
                name: "GuildChannelEvent");

            migrationBuilder.DropTable(
                name: "GuildChannel");
        }
    }
}
